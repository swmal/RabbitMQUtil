using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
//using RabbitMQUtil.Authentication;
using RabbitMQUtil.Logging;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil
{
    public class RabbitMqFactory
    {
        private readonly RabbitMqLogger _logger;

        public RabbitMqFactory()
            : this(RabbitMqLogger.NullLogger)
        {
            
        }

        public RabbitMqFactory(RabbitMqLogger logger)
        {
            _logger = logger;
        }

        public virtual ConnectionFactory GetConnectionFactory(QueueEndpoint endpoint)
        {
            _logger.Log("Creating connection factory");
            _logger.Log("Hostname: '" + (endpoint.Host ?? "") + "'");
            var factory = new ConnectionFactory()
                       {
                           HostName = endpoint.Host
                       };
            _logger.Log("VirtualHostName: '" + (endpoint.VirtualHost ?? "") + "'");
            if (!string.IsNullOrEmpty(endpoint.VirtualHost))
            {
                _logger.Log("Setting virtual host");
                factory.VirtualHost = endpoint.VirtualHost;
            }
            _logger.Log("Port: " + (endpoint.Port.HasValue ? endpoint.Port.Value.ToString() : "[not set]"));
            if(endpoint.Port.HasValue)
            {
                factory.Port = endpoint.Port.Value;
            }
            
            if (!string.IsNullOrEmpty(endpoint.User))
            {
                _logger.Log("Setting user and password");
                factory.UserName = endpoint.User;
                factory.Password = endpoint.Password;
            }
            return factory;
        }

        public virtual QueueingBasicConsumerWrapper GetBasicConsumer(IModel channel, IConnection connection)
        {
            var consumer = new QueueingBasicConsumer(channel);
            return new QueueingBasicConsumerWrapper(consumer, connection, channel);
        }
    }
}
