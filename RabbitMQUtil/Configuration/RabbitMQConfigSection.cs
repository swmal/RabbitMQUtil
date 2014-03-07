using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class RabbitMqConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("endpoints")]
        public QueueEndpointCollection Endpoints
        {
            get { return this["endpoints"] as QueueEndpointCollection; }
            set { this["endpoints"] = value; }
        }
    }
}
