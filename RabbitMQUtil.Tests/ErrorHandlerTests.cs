using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class ErrorHandlerTests : SenderTestBase
    {
        private ErrorHandler _errorHandler;
        private ReceivedMessage _message;

        [TestInitialize]
        public void Setup()
        {
            MessageData = Encoding.UTF8.GetBytes("hej");
            ChannelConfigurator = new ChannelConfigurator();
            Exchange = null;
            MessageProperties = new MessageProperties() { MessageId = MessageId };
            _message = new ReceivedMessage(MessageData, GetExchangeName(), MessageProperties);
        }

        [TestMethod]
        public void HandleErrorShouldCallBasicPublishWithSuppliedArguments()
        {
            SetupRabbitMqFactoryDefault();
            SetupErrorConfig(true, "errorqueue");
            var errorHandler = new ErrorHandler(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator, RabbitMqLogger.NullLogger);
            errorHandler.HandleError(_message, "testendpoint");

            Model.AssertWasCalled(x => x.BasicPublish(string.Empty, QueueEndpoint.Subscription.ErrorHandling.ErrorQueueName, Properties, MessageData));
        }
    }
}
