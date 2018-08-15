using Jankcat.VisualCompare.Lib.Browsers;
using Jankcat.VisualCompare.Lib.TestCaseManagers;
using Jankcat.VisualCompare.Lib.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jankcat.VisualCompare.Worker
{
    class Program
    {
        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            // Setup cancel key (Ctrl-C)
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            // Initialize the rabbit listeners
            Console.WriteLine("[RABBIT][JIRA] Listener starting");
            var factory = RabbitUtils.GetRabbitConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "JIRA",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueDeclare(queue: "URL",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                
                var jiraConsumer = new EventingBasicConsumer(channel);
                jiraConsumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(String.Format("[RABBIT][JIRA] Received: {0}", message));
                    if (string.IsNullOrWhiteSpace(message)) return;
                    // Send this ticket to the Orchestrator
                    JiraAsync(message).GetAwaiter().GetResult();
                };
                channel.BasicConsume(queue: "JIRA",
                                     autoAck: true,
                                     consumer: jiraConsumer);

                var urlConsumer = new EventingBasicConsumer(channel);
                urlConsumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(String.Format("[RABBIT][URL] Received: {0}", message));
                    if (string.IsNullOrWhiteSpace(message)) return;
                    var urls = (Models.UrlRequest)JsonConvert.DeserializeObject(message);
                    // Send this ticket to the Orchestrator
                    UrlAsync(urls).GetAwaiter().GetResult();
                };
                channel.BasicConsume(queue: "URL",
                                     autoAck: true,
                                     consumer: urlConsumer);

                Console.WriteLine("[RABBIT][JIRA] Listener started");

                // Wait for the Ctrl-C to come through
                _quitEvent.WaitOne();
                Console.WriteLine("[RABBIT][JIRA] Listener stopping...");
            }
        }

        private static async Task JiraAsync(string ticket)
        {
            // JIRA
            var jiraHost = Environment.GetEnvironmentVariable("VISDIFF_JIRA_HOST");
            var jiraUser = Environment.GetEnvironmentVariable("VISDIFF_JIRA_USER");
            var jiraKey = Environment.GetEnvironmentVariable("VISDIFF_JIRA_KEY");
            var jira = JIRAUtils.Create(jiraHost, jiraUser, jiraKey);

            // Browser
            var browser = new PuppeteerBrowser();
            var tcManager = new DefaultTestCaseManager(browser)
            {
                ProdBaseUrl = "http://example.com",
                StageBaseUrl = "http://stage.example.com"
            };

            // GO!
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Received: {0}", ticket));
            await OrchestrationUtils.RunJiraDiff(jira, tcManager, ticket);
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Completed: {0}", ticket));
        }

        private static async Task UrlAsync(Models.UrlRequest ticket)
        {
            // GO!
            Console.WriteLine(String.Format("[RABBIT][URL-ASYNC] Received: {0}", ticket));
            Console.WriteLine(String.Format("[RABBIT][URL-ASYNC] Completed: {0}", ticket));
        }
    }
}
