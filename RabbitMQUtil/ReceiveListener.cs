using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;
using System.IO;


namespace RabbitMQUtil
{
    public class ReceiveListener : Receiver, IReceiveListener
    {
        private QueueingBasicConsumerWrapper _consumer = null;
        private bool _isStarted = false;

        public ReceiveListener(
            IQueueEndpointProvider queueEndpointProvider, 
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfigurator,
            IErrorHandler errorHandler
            )
            : this(queueEndpointProvider, rabbitMqFactory, channelConfigurator, errorHandler, RabbitMqLogger.NullLogger)
        {

        }

        public ReceiveListener(
            IQueueEndpointProvider queueEndpointProvider,
            RabbitMqFactory rabbitMqFactory,
            ChannelConfigurator channelConfigurator,
            IErrorHandler errorHandler,
            RabbitMqLogger logger)
            : base(queueEndpointProvider, rabbitMqFactory, channelConfigurator, errorHandler, logger)
        {

        }

        public virtual void Start(string endpointName)
        {
            if (Receivers.Count == 0)
                throw new InvalidOperationException(
                    "ReceiveListener cannot be started without receivers. Register at least 1 receiver by calling AddReceiver(IReceiver receiver)");
            _isStarted = true;
            var endpoint = QueueEndpointProvider.GetEndpointByName(endpointName);
            EndpointName = endpointName;
            ValidateEndpoint(endpoint);
            var subscription = endpoint.Subscription;
            var factory = RabbitMqFactory.GetConnectionFactory(endpoint);
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    ChannelConfigurator.ConfigureErrorHandling(endpoint, channel);
                }
                using (var channel = connection.CreateModel())
                {
                    ChannelConfigurator.ConfigureQueue(endpoint, channel);
                    _consumer = RabbitMqFactory.GetBasicConsumer(channel, connection);
                    channel.BasicConsume(endpoint.RoutingKey, subscription.NoAck, _consumer.GetConsumer());
                    try
                    {
                        while (_isStarted)
                        {
                            var ea =
                                (BasicDeliverEventArgs) _consumer.Dequeue();
                            HandleReceivedMessage(channel, subscription, ea);
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        // This try-catch is not as ugly as it may seem;)
                        // http://www.rabbitmq.com/releases/rabbitmq-dotnet-client/v2.6.1/rabbitmq-dotnet-client-2.6.1-client-htmldoc/html/type-RabbitMQ.Util.SharedQueue.html#method-M:RabbitMQ.Util.SharedQueue.Dequeue
                        var sb = new StringBuilder();
                        sb.AppendLine("An EndOfStreamException was caught.");
                        sb.AppendLine("This behaviour is expected when a connection/channel is closed by the client.");
                        Logger.Log(sb.ToString());
                    }
                }
            }
        }

        public virtual void Stop()
        {
            _isStarted = false;
            _consumer.Close();
        }


        public Thread StartInThread(string endpointName)
        {
            var thread = new Thread(() => Start(endpointName));
            thread.Start();
            return thread;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
