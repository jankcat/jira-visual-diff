using Jankcat.VisualCompare.Lib.Browsers;
using Jankcat.VisualCompare.Lib.TestCaseManagers;
using Jankcat.VisualCompare.Lib.Utilities;
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

            // Initialize the JIRA listener
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
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
                                     consumer: consumer);

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
            var user = Environment.GetEnvironmentVariable("VISDIFF_GRID_USER");
            var apiKey = Environment.GetEnvironmentVariable("VISDIFF_GRID_KEY");
            var host = Environment.GetEnvironmentVariable("VISDIFF_GRID_HOST");
            var opts = SauceLabsBrowser.GetDefaultBrowserOptions(false);
            opts = SeleniumBrowser.AddCredentials(opts, user, apiKey);
            var browser = new SauceLabsBrowser(opts, host);

            var tcManager = new DefaultTestCaseManager(browser);

            // GO!
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Received: {0}", ticket));
            await OrchestrationUtils.RunJiraDiff(jira, tcManager, ticket);
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Completed: {0}", ticket));
        }
    }
}
