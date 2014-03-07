using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class WhenGetConnectionFactoryIsCalledAndExistingUser
    {
        private QueueEndpoint _endpoint = new QueueEndpoint() { User = "un", Password = "pw" };

        [TestMethod]
        public void UserShouldBeSet()
        {
            var factory = new RabbitMqFactory();
            var connectionFactory = factory.GetConnectionFactory(_endpoint);
            connectionFactory.UserName.Should().Be("un");
        }

        [TestMethod]
        public void PasswordShouldBeSet()
        {
            var factory = new RabbitMqFactory();
            var connectionFactory = factory.GetConnectionFactory(_endpoint);
            connectionFactory.Password.Should().Be("pw");
        }
    }

    [TestClass]
    public class WhenConnectionFactoryIsCalledWithVhostSet
    {
        private QueueEndpoint _endpoint = new QueueEndpoint() { User = "un", Password = "pw", VirtualHost = "vhost" };

        [TestMethod]
        public void VhostShouldBeSet()
        {
            var factory = new RabbitMqFactory();
            var connectionFactory = factory.GetConnectionFactory(_endpoint);
            connectionFactory.VirtualHost.Should().Be("vhost");
        }
    }

    [TestClass]
    public class WhenConnectionFactoryIsCalledWithNoVhostSet
    {
        private QueueEndpoint _endpoint = new QueueEndpoint();

        [TestMethod]
        public void VhostShouldBeASlash()
        {
            var factory = new RabbitMqFactory();
            var connectionFactory = factory.GetConnectionFactory(_endpoint);
            connectionFactory.VirtualHost.Should().Be("/");
        }
    }
}
