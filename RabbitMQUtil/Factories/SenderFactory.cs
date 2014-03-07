using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace RabbitMQUtil.Factories
{
    public static class SenderFactory
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

        public static ISender CreateSender()
        {
            return GetContainer().Resolve<ISender>();
        }
    }
}
