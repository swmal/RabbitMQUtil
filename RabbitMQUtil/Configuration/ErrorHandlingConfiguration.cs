using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class ErrorHandlingConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("errorQueueName")]
        public string ErrorQueueName
        {
            get { return this["errorQueueName"] as string; }
            set { this["errorQueueName"] = value; }
        }

        [ConfigurationProperty("enableErrorQueue", DefaultValue = false)]
        public bool EnableErrorQueue
        {
            get { return (bool)this["enableErrorQueue"]; }
            set { this["enableErrorQueue"] = value; }
        }

        [ConfigurationProperty("rethrowExceptions", DefaultValue = true)]
        public bool RethrowExceptions
        {
            get { return (bool)this["rethrowExceptions"]; }
            set { this["rethrowExceptions"] = value; }
        }
    }
}
