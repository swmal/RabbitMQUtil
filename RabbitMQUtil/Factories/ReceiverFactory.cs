using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace RabbitMQUtil.Factories
{
    public static class ReceiverFactory
    {
        private static IWindsorContainer _container;

        private static IWindsorContainer GetContainer()
        {
            if (_container != null) return _container;
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());
            _container = container;
            return _container;
        }

        public static IReceiveListener CreateReceiveListener()
        {
            return GetContainer().Resolve<IReceiveListener>();
        }

        public static NoneBlockingReceiver CreateNonBlockingReceiver()
        {
            return GetContainer().Resolve<NoneBlockingReceiver>();
        }

        public static IReceiver CreateActionReceiver(Action<ReceivedMessage> action)
        {
            return new ReceiverImpl(action);
        }
    }
}
