using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class QueueEndpoint : ConfigurationSection
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("host", DefaultValue = "localhost")]
        public string Host
        {
            get { return this["host"].ToString(); }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port")]
        public int? Port
        {
            get { 
                if(this["port"] != null)
                    return (int)this["port"];
                return default(int?);    
            }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("user", DefaultValue = "")]
        public string User
        {
            get { return this["user"].ToString(); }
            set { this["user"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "")]
        public string Password
        {
            get { return this["password"].ToString(); }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("virtualHost", DefaultValue = "")]
        public string VirtualHost
        {
            get { return this["virtualHost"].ToString(); }
            set { this["virtualHost"] = value; }
        }

        [ConfigurationProperty("routingKey")]
        public string RoutingKey
        {
            get { return this["routingKey"].ToString(); }
            set { this["routingKey"] = value; }
        }

        [ConfigurationProperty("exchange", IsRequired = false)]
        public ExchangeConfiguration Exchange
        {
            get { return this["exchange"] as ExchangeConfiguration; }
            set { this["exchange"] = value; }
        }

        [ConfigurationProperty("subscription", IsRequired = false)]
        public SubscriptionConfiguration Subscription
        {
            get { return this["subscription"] as SubscriptionConfiguration; }
            set { this["subscription"] = value; }
        }

        [ConfigurationProperty("pubSubType", IsRequired = true)]
        public PubSubType PubSubType
        {
            get { return (Configuration.PubSubType) Enum.Parse(typeof (PubSubType), this["pubSubType"].ToString(), true); }
            set { this["pubSubType"] = value; }
        }
    }
}
