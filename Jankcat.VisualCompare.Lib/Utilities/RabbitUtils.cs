using RabbitMQ.Client;
using System;

namespace Jankcat.VisualCompare.Lib.Utilities
{
    public static class RabbitUtils
    {
        public static ConnectionFactory GetRabbitConnection(string host = "", string user = "", string pass = "")
        {
            var envHost = Environment.GetEnvironmentVariable("VISDIFF_RABBIT_HOST");
            var resolvedHost = (!String.IsNullOrWhiteSpace(host)) ? host : (!String.IsNullOrWhiteSpace(envHost)) ? envHost : "localhost";
            var envUser = Environment.GetEnvironmentVariable("VISDIFF_RABBIT_USER");
            var resolvedUser = (!String.IsNullOrWhiteSpace(user)) ? user : (!String.IsNullOrWhiteSpace(envUser)) ? envUser : "guest";
            var envPass = Environment.GetEnvironmentVariable("VISDIFF_RABBIT_PASS");
            var resolvedPass = (!String.IsNullOrWhiteSpace(pass)) ? pass : (!String.IsNullOrWhiteSpace(envPass)) ? envPass : "guest";
            return new ConnectionFactory() { HostName = resolvedHost, UserName = resolvedUser, Password = resolvedPass };
        }
    }
}
