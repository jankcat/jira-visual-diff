using RabbitMQ.Client;
using System;

namespace CM.QA.Tools.VisualCompare.Lib.Utilities
{
    public static class RabbitUtil
    {
        public static ConnectionFactory GetRabbitConnection()
        {
            var host = Environment.GetEnvironmentVariable("CMQA_VISUAL_RABBIT_HOST");
            if (String.IsNullOrWhiteSpace(host)) host = "localhost";
            var user = Environment.GetEnvironmentVariable("CMQA_VISUAL_RABBIT_USER");
            if (String.IsNullOrWhiteSpace(user)) user = "guest";
            var pass = Environment.GetEnvironmentVariable("CMQA_VISUAL_RABBIT_PASS");
            if (String.IsNullOrWhiteSpace(pass)) pass = "guest";
            return new ConnectionFactory() { HostName = host, UserName = user, Password = pass };
        }
    }
}
