using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace RabbitMQUtil.Configuration
{
    public class SubscriptionConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("queueName")]
        public string QueueName
        {
            get { return this["queueName"] as string; }
            set { this["queueName"] = value; }
        }

        [ConfigurationProperty("durable", DefaultValue = false)]
        public bool Durable
        {
            get { return (bool)this["durable"]; }
            set { this["durable"] = value; }
        }

        [ConfigurationProperty("noAck")]
        public bool NoAck
        {
            get
            {
                var retVal = this["noAck"];
                return retVal != null && bool.Parse(this["noAck"].ToString());
            }
            set { this["noAck"] = value; }
        }

        [ConfigurationProperty("exchangeBindings")]
        public ExchangeBindingCollection ExchangeBindings
        {
            get { return this["exchangeBindings"] as ExchangeBindingCollection; }
            set { this["exchangeBindings"] = value; }
        }

        [ConfigurationProperty("errorConfig")]
        public ErrorHandlingConfiguration ErrorHandling
        {
            get { return this["errorConfig"] as ErrorHandlingConfiguration; }
            set { this["errorConfig"] = value; }
        }
    }
}
