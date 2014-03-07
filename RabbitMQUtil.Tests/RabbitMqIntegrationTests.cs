using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class RabbitMqIntegrationTests
    {
        private QueueEndpointProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new QueueEndpointProvider();
        }

        [TestMethod]
        public void ProviderShouldReadEndpointFromConfig()
        {
            var result = _provider.GetEndpointByName("testendpoint");
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void ProviderShouldReadEndpointNameFromConfig()
        {
            var result = _provider.GetEndpointByName("testendpoint");
            result.Name.Should().Be("testendpoint");
        }

        [TestMethod]
        public void ProviderShouldReadHostFromConfig()
        {
            var result = _provider.GetHostName();
            result.Should().Be("localhost");
        }

        [TestMethod]
        public void ProviderShouldReadEndpointRoutingKeyFromConfig()
        {
            var result = _provider.GetEndpointByName("testendpoint");
            result.RoutingKey.Should().Be("testqueue");
        }

        [TestMethod]
        public void AutoAckshouldBeSetFromConfig()
        {
            var result = _provider.GetEndpointByName("noAckIsSet");
            result.Subscription.NoAck.Should().BeTrue();
        }

        [TestMethod]
        public void ExchangeNameShouldBeSetFromConfig()
        {
            var result = _provider.GetEndpointByName("exchangeNameIsSet");
            result.Exchange.Name.Should().Be("ex1");
        }

        [TestMethod]
        public void PubSubTypeShouldBeSetFromConfig()
        {
            var result = _provider.GetEndpointByName("pubSubTypeIsSubscribe");
            result.PubSubType.Should().Be(PubSubType.Subscribe);
        }

        [TestMethod]
        public void ExchangeTypeShouldBeSetFromConfiguration()
        {
            var result = _provider.GetEndpointByName("exchangeTypeIsFanout");
            result.Exchange.Type.Should().Be(ExchangeType.Fanout);;
        }

        [TestMethod]
        public void ExchangeBindingsShouldBeReadFromConfiguration()
        {
            var result = _provider.GetEndpointByName("subscriptionWithExchangeBindings");
            result.Subscription.ExchangeBindings[0].Name.Should().Be("e1");
            result.Subscription.ExchangeBindings[0].Type.Should().Be(ExchangeType.Fanout);
            result.Subscription.ExchangeBindings[0].RoutingKey.Should().Be("r1");
            result.Subscription.ExchangeBindings[0].DeclareExchange.Should().BeTrue();
        }

        [TestMethod]
        public void ExchangeBindingsDeclareExchangeShouldBeFalseByDefault()
        {
            var result = _provider.GetEndpointByName("subscriptionWithExchangeBindingWhereDeclareExchangeIsMissing");
            result.Subscription.ExchangeBindings[0].DeclareExchange.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReadErrorConfigFromConfiguration()
        {
            var result = _provider.GetEndpointByName("errorConfigIsSet");
            result.Subscription.ErrorHandling.Should().NotBeNull();
            result.Subscription.ErrorHandling.ErrorQueueName.Should().Be("error.queue");
            result.Subscription.ErrorHandling.EnableErrorQueue.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldReadHostAndUserDataFromConfiguration()
        {
            var result = _provider.GetEndpointByName("hostAndUserData");
            result.Host.Should().Be("localhost");
            result.Port.Should().Be(123);
            result.User.Should().Be("usr");
            result.Password.Should().Be("pwd");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ProviderShouldThrowExceptionIfNameIsNotInConfig()
        {
            _provider.GetEndpointByName("an.endpoint.that.does.not.exist.in.config.file");
        }
    }
}
