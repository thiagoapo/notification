﻿

namespace Notification.Infrastructure.MQSettings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string ExchangeName { get; set; }
    }
}
