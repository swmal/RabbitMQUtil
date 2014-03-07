using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class IOCTests
    {
        [TestMethod]
        public void SenderCanBeResolved()
        {
            var windsorContainer = new WindsorContainer();
            windsorContainer.Install(FromAssembly.Named("RabbitMQUtil"));
            var sender = windsorContainer.Resolve<ISender>();
            Assert.IsInstanceOfType(sender, typeof(Sender));
        }

        [TestMethod]
        public void ReceiveListenerCanBeResolved()
        {
            var windsorContainer = new WindsorContainer();
            windsorContainer.Install(FromAssembly.Named("RabbitMQUtil"));
            var sender = windsorContainer.Resolve<IReceiveListener>();
            Assert.IsInstanceOfType(sender, typeof(ReceiveListener));
        }
    }
}
