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
            var factory = RabbitUtil.GetRabbitConnection();
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
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Received: {0}", ticket));
            await NNAHUtils.RunJiraDiff(ticket);
            Console.WriteLine(String.Format("[RABBIT][JIRA-ASYNC] Completed: {0}", ticket));
        }
    }
}