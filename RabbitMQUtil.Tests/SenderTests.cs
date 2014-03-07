using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using Rhino.Mocks;
//using RabbitMQUtil.Authentication;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class SenderTests : SenderTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            MessageData = Encoding.UTF8.GetBytes("hej");
            ChannelConfigurator = new ChannelConfigurator();
            Exchange = null;
            MessageProperties = new MessageProperties() { MessageId = MessageId };
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SendShouldThrowIfMessagePropertiesIsNull()
        {
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, null);
        }

        [TestMethod]
        public void SendShouldCallBasicPublishWithSuppliedArguments()
        {
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Model.AssertWasCalled(x => x.BasicPublish(GetExchangeName(), QueueEndpoint.RoutingKey, Properties, MessageData));
        }

        [TestMethod]
        public void SendShouldSetMessageIdOnProperties()
        {
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Assert.AreEqual(MessageId, Properties.MessageId);
        }

        [TestMethod]
        public void SendShouldSetCorrelationIdOnProperties()
        {
            MessageProperties.CorrelationId = "asdfklöj";
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Assert.AreEqual(MessageProperties.CorrelationId, Properties.CorrelationId);
        }

        [TestMethod]
        public void SendShouldSetContentEncodingOnProperties()
        {
            MessageProperties.ContentEncoding = "asdfklöj";
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Assert.AreEqual(MessageProperties.ContentEncoding, Properties.ContentEncoding);
        }

        [TestMethod]
        public void SendShouldSetContentTypeOnProperties()
        {
            MessageProperties.ContentType = "asdfklöj";
            SetupRabbitMqFactoryDefault();
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Assert.AreEqual(MessageProperties.ContentType, Properties.ContentType);
        }

        [TestMethod]
        public void SendShouldUseExchangeFromConfig()
        {
            const string exchange = "abc";
            SetupRabbitMqFactoryWithExchange(exchange);
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Model.AssertWasCalled(x => x.BasicPublish(exchange, QueueEndpoint.RoutingKey, Properties, MessageData));
        }

        [TestMethod]
        public void SendShouldUseRoutingKeyIfSpecified()
        {
            const string exchange = "abc";
            SetupRabbitMqFactoryWithExchange(exchange);
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties, "my.routingkey");

            Model.AssertWasCalled(x => x.BasicPublish(exchange, "my.routingkey", Properties, MessageData));
        }

        [TestMethod]
        public void SendShouldCallExchangeDeclare()
        {
            const string exchange = "abc";
            SetupRabbitMqFactoryWithExchange(exchange);
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Model.AssertWasCalled(x => x.ExchangeDeclare(exchange, "fanout", Exchange.Durable));
        }

        [TestMethod]
        public void SendShouldCallExchangeDeclareIfExchangeIsEmpty()
        {
            var exchange = string.Empty;
            SetupRabbitMqFactoryWithExchange(exchange);
            var sender = new Sender(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator);
            sender.Send("testendpoint", MessageData, MessageProperties);

            Model.AssertWasNotCalled(x => x.ExchangeDeclare(exchange, "fanout"));
        }
    }
}
