using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil.Tests
{
    public abstract class ReceiverTestsBase
    {
        protected class ReceiverStub : IReceiver
        {
            private readonly ReceiveListener _listener;

            public ReceiverStub()
            {

            }

            public ReceiverStub(ReceiveListener listener)
            {
                _listener = listener;
            }

            public ReceivedMessage Message { get; private set; }

            public virtual void Receive(ReceivedMessage message)
            {
                Message = message;
                if (_listener != null)
                {
                    _listener.Stop();
                }
            }
        }

        protected class ReceiverThatThrowsException : ReceiverStub
        {
            public ReceiverThatThrowsException()
                : base()
            {
                
            }

            public ReceiverThatThrowsException(ReceiveListener listener)
                : base(listener)
            {

            }

            public override void Receive(ReceivedMessage message)
            {
                base.Receive(message);
                throw new InvalidOperationException("an exception");
            }
        }

        protected enum CorrelationIdState
        {
            DoNotUse,
            Use
        }

        protected const string MessageId = "123";
        protected const string CorrelationId = "456";
        protected const string ContentType = "contentType";
        protected const string ContentEncoding = "contentEncoding";
        protected const string HostName = "testhost";
        protected QueueEndpoint QueueEndpoint { get; set; }
        protected IModel Model { get; set; }
        protected QueueingBasicConsumer Consumer;
        protected ChannelConfigurator ChannelConfigurator;
        protected byte[] MessageData;
        protected IQueueEndpointProvider QueueEndpointProvider;
        protected RabbitMqFactory RabbitMqFactory;
        protected IBasicProperties BasicProperties;
        protected QueueingBasicConsumerWrapper ConsumerWrapper;
        protected IConnection Connection;
        protected ErrorHandler ErrorHandler;

        protected void InternalSetup()
        {
            MessageData = Encoding.UTF8.GetBytes("hej");
            ChannelConfigurator = MockRepository.GenerateStub<ChannelConfigurator>();
        }

        protected void SetupEndpoint(string name, string virtualHost, string routingKey)
        {
            QueueEndpoint = new QueueEndpoint() { Name = name, RoutingKey = routingKey, VirtualHost = virtualHost, PubSubType = PubSubType.Subscribe };
        }

        protected void SetupModel(QueueEndpoint endPoint, IBasicProperties properties, byte[] data)
        {
            Model = MockRepository.GenerateStub<IModel>();
            Model.Stub(x => x.QueueDeclare(endPoint.RoutingKey, false, false, false, null))
                .Return(new QueueDeclareOk(endPoint.RoutingKey, 1, 1));
            Model.Stub(x => x.BasicConsume(endPoint.RoutingKey, true, Consumer))
                .Return(null);
        }

        protected void SetupModelBasicGet(QueueEndpoint endPoint, BasicGetResult result)
        {
            Model.Stub(x => x.BasicGet(endPoint.RoutingKey, false)).Return(result);
        }

        protected void SetupQueueEndpointProvider(string endpointName, string virtualHost, string queueName)
        {
            SetupEndpoint(endpointName, virtualHost, queueName);
            QueueEndpointProvider = MockRepository.GenerateStub<IQueueEndpointProvider>();
            QueueEndpointProvider
                .Stub(x => x.GetEndpointByName(endpointName))
                .Return(QueueEndpoint);
            QueueEndpointProvider
                .Stub(x => x.GetHostName())
                .Return(HostName);

        }

        private ConnectionFactory CreateFactoryMock(QueueEndpoint endPoint, IBasicProperties properties, byte[] data)
        {
            SetupModel(endPoint, properties, data);
            Connection = MockRepository.GenerateStub<IConnection>();
            Connection
                .Stub(x => x.CreateModel())
                .Return(Model);

            var factory = MockRepository.GenerateStub<ConnectionFactory>();
            factory.Stub(x => x.CreateConnection())
                .Return(Connection);
            return factory;

        }

        protected IBasicProperties SetupBasicProperties(CorrelationIdState correlationIdState)
        {
            BasicProperties = MockRepository.GenerateStub<IBasicProperties>();
            BasicProperties.MessageId = MessageId;
            BasicProperties.ContentType = ContentType;
            BasicProperties.ContentEncoding = ContentEncoding;
            if (correlationIdState == CorrelationIdState.Use) BasicProperties.CorrelationId = CorrelationId;
            return BasicProperties;
        }

        protected void SetupRabbitMqFactory()
        {
            RabbitMqFactory = MockRepository.GenerateStub<RabbitMqFactory>();
            RabbitMqFactory
                .Stub(x => x.GetConnectionFactory(QueueEndpoint))
                .Return(CreateFactoryMock(QueueEndpoint, BasicProperties, MessageData));
            RabbitMqFactory
                .Stub(x => x.GetBasicConsumer(Model, Connection))
                .Return(ConsumerWrapper);
        }

        protected void CreateConsumerMock(CorrelationIdState correlationIdState, string routingKey)
        {
            Consumer = MockRepository.GenerateStub<QueueingBasicConsumer>();
            ConsumerWrapper = MockRepository.GenerateStub<QueueingBasicConsumerWrapper>(Consumer, Connection, Model);
            ConsumerWrapper.Stub(x => x.GetConsumer())
                .Return(Consumer);
            SetupBasicProperties(correlationIdState);
            ConsumerWrapper.Stub(x => x.Dequeue())
                .Return(new BasicDeliverEventArgs("", 0, false, "ex1", routingKey, BasicProperties, MessageData));
        }

        protected void CreateConsumerMock(CorrelationIdState correlationIdState)
        {
           CreateConsumerMock(correlationIdState, string.Empty);
        }

        protected void SetupErrorHandler(bool rethrowExceptions)
        {
            ErrorHandler = MockRepository.GenerateStub<ErrorHandler>(
                QueueEndpointProvider,
                RabbitMqFactory,
                ChannelConfigurator,
                MockRepository.GenerateStub<RabbitMqLogger>()
                );
            ErrorHandler.Stub(x => x.ShouldRethrowExceptions(this.QueueEndpoint.Name))
                        .Return(rethrowExceptions);
        }
    }
}
