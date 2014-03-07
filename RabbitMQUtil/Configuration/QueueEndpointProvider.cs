using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class QueueEndpointProvider : IQueueEndpointProvider
    {
        public QueueEndpoint GetEndpointByName(string name)
        {
            var section = ConfigurationManager.GetSection("rabbitMq") as RabbitMqConfigSection;
            if (section != null)
            {
                for (var x = 0; x < section.Endpoints.Count; x++)
                {
                    if (section.Endpoints[x].Name == name)
                    {
                        return section.Endpoints[x];
                    }
                }
            }
            throw new ArgumentException("endpoint " + name + " was not found in configuration");
        }

        public string GetHostName()
        {
            return ConfigurationManager.AppSettings["RabbitMq.Host"];
        }
    }
}
