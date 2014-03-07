using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class ExchangeConfiguration : ConfigurationElement
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
            get { return (ExchangeType) Enum.Parse(typeof (ExchangeType), this["type"].ToString(), true); }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("durable", DefaultValue = true)]
        public bool Durable
        {
            get { return (bool)this["durable"]; }
            set { this["durable"] = value; }
        }
    }
}
