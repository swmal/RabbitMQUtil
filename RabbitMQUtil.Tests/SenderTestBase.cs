using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    public abstract class SenderTestBase
    {
        protected const string MessageId = "123";
        protected IModel Model;
        protected MessageProperties MessageProperties;
        protected IBasicProperties Properties;
        protected ChannelConfigurator ChannelConfigurator;
        protected QueueEndpoint QueueEndpoint;
        protected IQueueEndpointProvider QueueEndpointProvider;
        protected ExchangeConfiguration Exchange;
        protected byte[] MessageData;
        protected RabbitMqFactory RabbitMqFactory;

        protected void SetupExchange(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Exchange = new ExchangeConfiguration() { Name = name, Durable = true };
            }
        }

        protected void SetupModel(QueueEndpoint endPoint, IBasicProperties properties, byte[] data)
        {
            Model = MockRepository.GenerateStub<IModel>();
            Model.Stub(x => x.QueueDeclare(endPoint.RoutingKey, false, false, false, null))
                .Return(new QueueDeclareOk(endPoint.RoutingKey, 1, 1));
            Model.Stub(x => x.CreateBasicProperties())
                .Return(properties);
            Model.Stub(x => x.BasicPublish(Exchange != null ? Exchange.Name : null, endPoint.RoutingKey, properties, data));
        }

        protected ConnectionFactory CreateFactoryMock(QueueEndpoint endPoint, IBasicProperties properties, byte[] data)
        {
            SetupModel(endPoint, properties, data);
            var connection = MockRepository.GenerateStub<IConnection>();
            connection
                .Stub(x => x.CreateModel())
                .Return(Model);

            var factory = MockRepository.GenerateStub<ConnectionFactory>();
            factory.Stub(x => x.CreateConnection())
                .Return(connection);
            return factory;
        }

        protected void SetupEndpoint(string virtualHost, string queueName, string exchangeName)
        {
            if (!string.IsNullOrWhiteSpace(exchangeName))
            {
                Exchange = new ExchangeConfiguration() { Name = exchangeName, Type = RabbitMQUtil.ExchangeType.Fanout };
            }
            QueueEndpoint = new QueueEndpoint()
            {
                VirtualHost = virtualHost,
                RoutingKey = queueName,
                Exchange = Exchange
            };
        }

        protected void SetupErrorConfig(bool enableErrorQueue, string errorQueueName)
        {
            QueueEndpoint.Subscription = new SubscriptionConfiguration
                                             {
                                                 ErrorHandling =
                                                     new ErrorHandlingConfiguration
                                                         {
                                                             EnableErrorQueue
                                                                 =
                                                                 enableErrorQueue,
                                                             ErrorQueueName
                                                                 =
                                                                 errorQueueName
                                                         }
                                             };
        }

        protected void SetupQueueEndpointProvider(string endpointName, string virtualHost, string queueName, string exchangeName)
        {
            SetupEndpoint(virtualHost, queueName, exchangeName);
            QueueEndpointProvider = MockRepository.GenerateStub<IQueueEndpointProvider>();
            QueueEndpointProvider
                .Stub(x => x.GetEndpointByName(endpointName))
                .Return(QueueEndpoint);
            QueueEndpointProvider
                .Stub(x => x.GetHostName())
                .Return("testhost");

        }

        protected void SetupRabbitMqFactory(string exchange)
        {
            SetupExchange(exchange);
            Properties = MockRepository.GenerateStub<IBasicProperties>();
            SetupQueueEndpointProvider("testendpoint", "vhost", "testqueue", exchange);
            RabbitMqFactory = MockRepository.GenerateStub<RabbitMqFactory>();
            RabbitMqFactory
                .Stub(x => x.GetConnectionFactory(QueueEndpoint))
                .Return(CreateFactoryMock(QueueEndpoint, Properties, MessageData));
        }

        protected void SetupRabbitMqFactoryDefault()
        {
            SetupRabbitMqFactory(string.Empty);
        }

        protected void SetupRabbitMqFactoryWithExchange(string exchangeName)
        {
            SetupRabbitMqFactory(exchangeName);
        }

        protected string GetExchangeName()
        {
            return Exchange != null ? Exchange.Name : "";
        }
    }
}
