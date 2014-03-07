using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil.Castle
{
    public class CastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IReceiveListener>().ImplementedBy<ReceiveListener>().LifestylePerThread());
            container.Register(Component.For<ISender>().ImplementedBy<Sender>());
            container.Register(Component.For<IQueueEndpointProvider>().ImplementedBy<QueueEndpointProvider>());
            container.Register(Component.For<RabbitMqFactory>().ImplementedBy<RabbitMqFactory>());
            container.Register(Component.For<ChannelConfigurator>().ImplementedBy<ChannelConfigurator>());
            container.Register(Component.For<NoneBlockingReceiver>().LifestyleTransient());
            container.Register(Component.For<RabbitMqLogger>().LifestyleSingleton());
            container.Register(Component.For<IErrorHandler>().ImplementedBy<ErrorHandler>());
        }
    }
}
