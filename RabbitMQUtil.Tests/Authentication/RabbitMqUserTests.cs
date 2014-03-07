using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace RabbitMQUtil.Tests.Authentication
{
    [TestClass]
    public class WhenUserAndPasswordExistsInConfig
    {

        [TestInitialize]
        public void Setup()
        {
            var appsettings = MockRepository.GenerateStub<AppSettingsWrapper>();
            appsettings.Stub(x => x.GetSetting("RabbitMq.Username")).Return("un");
            appsettings.Stub(x => x.GetSetting("RabbitMq.Password")).Return("pwd");
            _userProvider = new RabbitMqUserProvider(appsettings);
        }

        [TestMethod]
        public void UsernameShouldBeSet()
        {
            var user = _userProvider.GetUser();
            user.UserName.Should().Be("un");
        }

        [TestMethod]
        public void PasswordShouldBeSet()
        {
            var user = _userProvider.GetUser();
            user.Password.Should().Be("pwd");
        }

        [TestMethod]
        public void IsEmptyShouldBeFalse()
        {
            var user = _userProvider.GetUser();
            user.IsEmpty.Should().BeFalse();
        }
    }

    [TestClass]
    public class WhenNoUserAndPasswordExistsInConfig
    {
        private RabbitMqUserProvider _userProvider;

        [TestInitialize]
        public void Setup()
        {
            var appsettings = MockRepository.GenerateStub<AppSettingsWrapper>();
            _userProvider = new RabbitMqUserProvider(appsettings);
        }

        [TestMethod]
        public void IsEmptyShouldBeTrue()
        {
            var user = _userProvider.GetUser();
            user.IsEmpty.Should().BeTrue();
        }
    }
}
