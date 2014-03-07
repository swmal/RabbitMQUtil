using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQUtil
{
    public class BasicPropertiesFactory
    {
        public virtual IBasicProperties Create(IModel channel, MessageProperties properties)
        {
            var basicProperties = channel.CreateBasicProperties();
            if(!string.IsNullOrWhiteSpace(properties.MessageId))
            {
                basicProperties.MessageId = properties.MessageId;
            }
            if(!string.IsNullOrWhiteSpace(properties.CorrelationId))
            {
                basicProperties.CorrelationId = properties.CorrelationId;
            }
            if(!string.IsNullOrWhiteSpace(properties.ContentType))
            {
                basicProperties.ContentType = properties.ContentType;
            }
            if(!string.IsNullOrWhiteSpace(properties.ContentEncoding))
            {
                basicProperties.ContentEncoding = properties.ContentEncoding;
            }
            basicProperties.DeliveryMode = (byte) properties.DeliveryMode;
            if(properties.Headers.Keys.Any())
            {
                basicProperties.Headers = new Dictionary<object, object>();
                foreach(var key in properties.Headers.Keys)
                {
                    basicProperties.Headers.Add(key, properties.Headers[key]);
                }
            }
            return basicProperties;
        }
    }
}
