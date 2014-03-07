using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class ExchangeBinding : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "Fanout")]
        public ExchangeType Type
        {
            get { return (ExchangeType)Enum.Parse(typeof(ExchangeType), this["type"].ToString(), true); }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("routingKey", DefaultValue = "")]
        public string RoutingKey
        {
            get { return this["routingKey"].ToString(); }
            set { this["routingKey"] = value; }
        }

        [ConfigurationProperty("declareExchange", DefaultValue = false)]
        public bool DeclareExchange
        {
            get { return (bool) this["declareExchange"]; }
            set { this["declareExchange"] = value;  }
        }
    }
}
