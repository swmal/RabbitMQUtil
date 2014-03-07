using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil
{
    public class NoneBlockingReceiver : Receiver
    {
        private readonly RabbitMqLogger _logger;
        private QueueingBasicConsumerWrapper _consumer = null;

        public NoneBlockingReceiver(
            IQueueEndpointProvider queueEndpointProvider, 
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfigurator,
            IErrorHandler errorHandler)
            : this(queueEndpointProvider, rabbitMqFactory, channelConfigurator, errorHandler, RabbitMqLogger.NullLogger)
        {
        }

        public NoneBlockingReceiver(
            IQueueEndpointProvider queueEndpointProvider,
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfigurator,
            IErrorHandler errorHandler,
            RabbitMqLogger logger)
            : base(queueEndpointProvider, rabbitMqFactory, channelConfigurator, errorHandler, logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This method checks for a message on the supplied endpoint
        /// </summary>
        /// <param name="endpointName">Name of the endpoint configuration</param>
        /// <returns>True if a message was found on the queue and handled, otherwise false</returns>
        public virtual bool CheckForMessage(string endpointName)
        {
            var retVal = false;
            var endpoint = QueueEndpointProvider.GetEndpointByName(endpointName);
            ValidateEndpoint(endpoint);
            EndpointName = endpointName;
            var subscription = endpoint.Subscription;
            var factory = RabbitMqFactory.GetConnectionFactory(endpoint);
            using (IConnection connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    ChannelConfigurator.ConfigureErrorHandling(endpoint, channel);
                }
                using (var channel = connection.CreateModel())
                {
                    ChannelConfigurator.ConfigureQueue(endpoint, channel);
                    _consumer = RabbitMqFactory.GetBasicConsumer(channel, connection);
                    var result = channel.BasicGet(subscription.QueueName, subscription.NoAck);
                    if (result != null)
                    {
                        HandleReceivedMessage(channel, subscription, result);
                        retVal = true;
                    }
                    _consumer.Close();
                }
            }
            
            return retVal;
        }
    }
}
