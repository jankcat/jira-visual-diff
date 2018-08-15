using System;
using System.Text;
using Jankcat.VisualCompare.Lib.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Jankcat.VisualCompare.API.Controllers
{
    [Route("api/visual/url")]
    public class UrlController : Controller
    {
        [HttpPost]
        public Models.UrlRequest Post()
        {
            if (!Request.Form.ContainsKey("url")) return null;
            var urls = Request.Form["url"];
            var request = new Models.UrlRequest
            {
                Urls = urls,
            };
            var requestString = JsonConvert.SerializeObject(request);

            var factory = RabbitUtils.GetRabbitConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "URL",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var body = Encoding.UTF8.GetBytes(requestString);
                channel.BasicPublish(exchange: "",
                                     routingKey: "URL",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine("[URL] Sent {0}", requestString);
            }
            return request;
        }
    }
}
