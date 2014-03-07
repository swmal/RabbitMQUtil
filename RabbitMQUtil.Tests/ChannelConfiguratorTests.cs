using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class ChannelConfiguratorTests
    {
        private QueueEndpoint _endpoint;
        private IModel _channel;

        [TestInitialize]
        public void Setup()
        {
            _channel = MockRepository.GenerateStub<IModel>();
        }

        [TestMethod]
        public void ShouldCallQueueDeclareAndQueueBindWhenPubSubTypeIsSubscribe()
        {
            const string queueName = "abc";
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                PubSubType = PubSubType.Subscribe,
                Subscription = new SubscriptionConfiguration()
                {
                    ExchangeBindings = new ExchangeBindingCollection()
                }
            };
            _endpoint.Subscription.ExchangeBindings.Add(
                new ExchangeBinding()
                    {
                        Name = "e1", 
                        RoutingKey = "rk"
                    });
            _channel.Stub(x => x.QueueDeclare()).Return(new QueueDeclareOk(queueName, 1, 1));
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);

            _channel.AssertWasCalled(x => x.QueueDeclare());
            _channel.AssertWasCalled(x => x.QueueBind(
                queueName,
                _endpoint.Subscription.ExchangeBindings[0].Name,
                _endpoint.Subscription.ExchangeBindings[0].RoutingKey));
        }

        [TestMethod]
        public void ShouldCallExchangeDeclarePubSubTypeIsSubscribeAndExchangeBindingShouldDeclare()
        {
            const string queueName = "abc";
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                PubSubType = PubSubType.Subscribe,
                Subscription = new SubscriptionConfiguration()
                {
                    ExchangeBindings = new ExchangeBindingCollection()
                }
            };
            _endpoint.Subscription.ExchangeBindings.Add(
                new ExchangeBinding()
                {
                    Name = "e1",
                    RoutingKey = "rk",
                    Type = ExchangeType.Direct,
                    DeclareExchange = true
                });
            _channel.Stub(x => x.QueueDeclare()).Return(new QueueDeclareOk(queueName, 1, 1));
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);

            var binding = _endpoint.Subscription.ExchangeBindings[0];
            _channel.AssertWasCalled(x => x.ExchangeDeclare(
                binding.Name,
                "direct"));
        }

        [TestMethod]
        public void ShouldNotCallQueueDeclareAndQueueBindWhenPubSubTypeIsPublish()
        {
            const string queueName = "abc";
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                Exchange = new ExchangeConfiguration() { Name = "ex1" },
                Subscription = new SubscriptionConfiguration() { QueueName = "asdf" },
                PubSubType = PubSubType.Publish
            };
            _channel.Stub(x => x.QueueDeclare()).Return(new QueueDeclareOk(queueName, 1, 1));
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);

            _channel.AssertWasNotCalled(x => x.QueueDeclare());
            _channel.AssertWasNotCalled(x => x.QueueBind(_endpoint.Subscription.QueueName, _endpoint.Exchange.Name, ""));
        }

        [TestMethod]
        public void ShouldCallQueueDeclareWithParametersWhenPubSubTypeIsSubscribeAndRoutingKeyIsSet()
        {
            const string queueName = "abc";
            var expectedResult = new QueueDeclareOk(queueName, 1, 1);
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                Exchange = new ExchangeConfiguration() { Name = "ex1" },
                PubSubType = PubSubType.Subscribe,
                RoutingKey = queueName,
                Subscription = new SubscriptionConfiguration() { QueueName = "abckduf", NoAck = false }
            };
            _endpoint.Subscription.ExchangeBindings.Add(new ExchangeBinding() { Name = "e1" });
            _channel
                .Stub(x => x.QueueDeclare())
                .Return(new QueueDeclareOk(queueName, 1, 1));
            _channel
                .Stub(x => x.QueueDeclare(_endpoint.Subscription.QueueName, _endpoint.Subscription.NoAck, false, false, null))
                .Return(expectedResult);
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);

            _channel.AssertWasCalled(x => x.QueueDeclare(_endpoint.Subscription.QueueName, _endpoint.Subscription.Durable, false, false, null));
            _channel.AssertWasCalled(x => x.QueueBind(expectedResult, _endpoint.Subscription.ExchangeBindings[0].Name, ""));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowExceptionIfPubSubTypeIsSubscribeAndNoSubscriptionConfigExists()
        {
            const string queueName = "abc";
            var expectedResult = new QueueDeclareOk(queueName, 1, 1);
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                Exchange = new ExchangeConfiguration() { Name = "ex1" },
                PubSubType = PubSubType.Subscribe,
                RoutingKey = queueName,
                Subscription = null
            };
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);
        }

        [TestMethod]
        public void ShouldNotCallExchangeDeclareIfExchangesNameIsEmpty()
        {
            _endpoint = new QueueEndpoint()
            {
                Name = "testendpoint",
                Exchange = new ExchangeConfiguration() { Name = string.Empty, Type = ExchangeType.Fanout},
                PubSubType = PubSubType.Publish,
                RoutingKey = "asdf",
                Subscription = null
            };
            var configurator = new ChannelConfigurator();
            configurator.ConfigureQueue(_endpoint, _channel);
            _channel.AssertWasNotCalled(x => x.ExchangeDeclare(_endpoint.Exchange.Name, _endpoint.Exchange.Type.ToString().ToLower()));
        }
    }
}
