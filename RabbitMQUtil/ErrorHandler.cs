using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IQueueEndpointProvider _queueEndpointProvider;
        private readonly RabbitMqFactory _rabbitMqFactory;
        private readonly ChannelConfigurator _channelConfiguratior;
        private readonly RabbitMqLogger _logger;

        public ErrorHandler(
            IQueueEndpointProvider queueEndpointProvider,
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfiguratior,
            RabbitMqLogger logger)
        {
            _queueEndpointProvider = queueEndpointProvider;
            _rabbitMqFactory = rabbitMqFactory;
            _channelConfiguratior = channelConfiguratior;
            _logger = logger;
        }

        public virtual bool ShouldRethrowExceptions(string endpointName)
        {
            var endpoint = _queueEndpointProvider.GetEndpointByName(endpointName);
            var errorConfig = endpoint.Subscription.ErrorHandling;
            if (errorConfig == null) return true;
            return errorConfig.RethrowExceptions;
        }

        public virtual void HandleError(ReceivedMessage message, string endpointName)
        {
            var endpoint = _queueEndpointProvider.GetEndpointByName(endpointName);
            var errorConfig = endpoint.Subscription.ErrorHandling;
            if (errorConfig == null || !errorConfig.EnableErrorQueue) return;
            _logger.Log("Endpoint " + endpointName + " read from config.");
            var factory = _rabbitMqFactory.GetConnectionFactory(endpoint);
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    _channelConfiguratior.ConfigureQueue(endpoint, channel);
                    message.Properties.DeliveryMode = DeliveryMode.Persistent;
                    var basicProperties = new BasicPropertiesFactory().Create(channel, message.Properties);
                    _logger.Log("Before ErrorHandler: Publish to error queue");
                    LogEndpointData(factory, errorConfig);
                    channel.BasicPublish(string.Empty, errorConfig.ErrorQueueName, basicProperties, message.Data);
                    _logger.Log("After ErrorHandler:Publish to error queue, bytes.Length: " + message.Data.Length);
                }
            }
        }

        private void LogEndpointData(ConnectionFactory factory, ErrorHandlingConfiguration errorConfig)
        {
            _logger.Log("Host: " + factory.HostName);
            _logger.Log("Virtual host: " + factory.VirtualHost);
            _logger.Log("ErrorQueue: '" + errorConfig.ErrorQueueName+ "'");
        }
    }
}
