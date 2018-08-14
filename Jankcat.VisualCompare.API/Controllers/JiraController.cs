using System;
using System.Text;
using Jankcat.VisualCompare.Lib.Utilities;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Jankcat.VisualCompare.API.Controllers
{
    [Route("api/visual/jira")]
    public class JiraController : Controller
    {
        [HttpGet("{id}")]
        public string Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return "No ticket number provided";
            var factory = RabbitUtils.GetRabbitConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "JIRA",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
                var body = Encoding.UTF8.GetBytes(id);
                channel.BasicPublish(exchange: "",
                                 routingKey: "JIRA",
                                 basicProperties: null,
                                 body: body);
                Console.WriteLine("[JIRA] Sent {0}", id);
            }
            return "Ticket accepted";
        }

        [HttpPost("{id}")]
        public void Post(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            var factory = RabbitUtils.GetRabbitConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "JIRA",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
                var body = Encoding.UTF8.GetBytes(id);
                channel.BasicPublish(exchange: "",
                                 routingKey: "JIRA",
                                 basicProperties: null,
                                 body: body);
                Console.WriteLine("[JIRA] Sent {0}", id);
            }
        }
    }
}
