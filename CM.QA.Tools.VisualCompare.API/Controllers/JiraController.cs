using System;
using System.Text;
using CM.QA.Tools.VisualCompare.Lib.Utilities;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CM.QA.Tools.VisualCompare.API.Controllers
{
    [Route("api/visual/jira")]
    public class JiraController : Controller
    {
        [HttpGet("{id}")]
        public string Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return "No ticket number provided";
            var factory = RabbitUtil.GetRabbitConnection();
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
            var factory = RabbitUtil.GetRabbitConnection();
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
