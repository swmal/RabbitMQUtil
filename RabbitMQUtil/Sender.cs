using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil
{
    public class Sender : ISender
    {
        private readonly IQueueEndpointProvider _queueEndpointProvider;
        private readonly RabbitMqFactory _rabbitMqFactory;
        private readonly ChannelConfigurator _channelConfiguratior;
        private readonly RabbitMqLogger _logger;

        public Sender(
            IQueueEndpointProvider queueEndpointProvider, 
            RabbitMqFactory rabbitMqFactory, 
            ChannelConfigurator channelConfiguratior)
            : this(queueEndpointProvider, rabbitMqFactory, channelConfiguratior, RabbitMqLogger.NullLogger)
        {
            
        }

        public Sender(
            IQueueEndpointProvider queueEndpointProvider,
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfiguratior,
            RabbitMqLogger logger
            )
        {
            _queueEndpointProvider = queueEndpointProvider;
            _rabbitMqFactory = rabbitMqFactory;
            _channelConfiguratior = channelConfiguratior;
            _logger = logger;
        }

        public virtual void Send(string endpointName, string data, MessageProperties properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            var bodyBytes = properties.GetEncoding().GetBytes(data);
            Send(endpointName, bodyBytes, properties);
        }

        public virtual void Send(string endpointName, byte[] data, MessageProperties properties)
        {
            var endpoint = _queueEndpointProvider.GetEndpointByName(endpointName);
            Send(endpointName, data, properties, endpoint.RoutingKey);
        }

        public virtual void Send(string endpointName, byte[] data, MessageProperties properties, string routingKey)
        {
            ValidateInput(endpointName, data, properties);
            var endpoint = _queueEndpointProvider.GetEndpointByName(endpointName);
            _logger.Log("Endpoint " + endpointName + " read from config.");
            var factory = _rabbitMqFactory.GetConnectionFactory(endpoint);
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    _channelConfiguratior.ConfigureQueue(endpoint, channel);
                    var basicProperties = new BasicPropertiesFactory().Create(channel, properties);
                    _logger.Log("Before Sender.Publish");
                    var exchangeName = GetExchangeName(endpoint);
                    LogEndpointData(factory, endpoint);
                    channel.BasicPublish(GetExchangeName(endpoint), routingKey, basicProperties, data);
                    _logger.Log("After Sender.Publish, bytes.Length: " + data.Length);
                }
            }
        }

        private void LogEndpointData(ConnectionFactory factory, QueueEndpoint endpoint)
        {
            _logger.Log("Host: " + factory.HostName);
            _logger.Log("Virtual host: " + factory.VirtualHost);
            _logger.Log("ExchangeName: '" + GetExchangeName(endpoint) + "'");
            _logger.Log("RoutingKey: '" + endpoint.RoutingKey + "'");
        }

        private static void ValidateInput(string endpointName, byte[] data, MessageProperties properties)
        {
            if(properties == null) throw new ArgumentNullException("properties");
        }

        private static string GetExchangeName(QueueEndpoint endpoint)
        {
            return endpoint.Exchange != null ? endpoint.Exchange.Name : string.Empty;
        }
    }
}
