using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace RabbitMQUtil.Configuration
{
    public class QueueEndpointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueEndpoint();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueEndpoint)element).Name;
        }

        public QueueEndpoint this[int index]
        {
            get { return BaseGet(index) as QueueEndpoint; }
        }
    }
}
