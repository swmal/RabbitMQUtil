using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class NonBlockingReceiverTests : ReceiverTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            InternalSetup();
            SetupReceiver();
        }

        private NoneBlockingReceiver _receiver;

        private void SetupReceiver()
        {
            base.SetupQueueEndpointProvider("testendpoint", "vhost", "testqueue");
            base.SetupBasicProperties(CorrelationIdState.DoNotUse);
            base.CreateConsumerMock(CorrelationIdState.DoNotUse);
            base.SetupRabbitMqFactory();
            base.SetupErrorHandler(false);
            _receiver = new NoneBlockingReceiver(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator, ErrorHandler);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ReceiverShouldThrowIfPubSubTypeIsPublish()
        {
            SetupReceiver();
            var receiver = new ReceiverStub();
            _receiver.AddReceiver(receiver);
            QueueEndpoint.PubSubType = PubSubType.Publish;
            _receiver.CheckForMessage("testendpoint");
        }

        [TestMethod]
        public void ReceiverShouldReturnFalseIfBasicGetReturnsNull()
        {
            SetupModelBasicGet(this.QueueEndpoint, null);
            _receiver.AddReceiver(new ReceiverStub());

            var result = _receiver.CheckForMessage("testendpoint");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReceiverShouldReturnTrueIfBasicGetReturnsAnInstance()
        {
            this.QueueEndpoint.Subscription = new SubscriptionConfiguration() { QueueName = "testqueue" };
            SetupModelBasicGet(this.QueueEndpoint, new BasicGetResult(1, false, "", "", 1, BasicProperties, MessageData));
            _receiver.AddReceiver(new ReceiverStub());

            var result = _receiver.CheckForMessage("testendpoint");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReceiverShouldReturnDispatchMessageToRegistredReceiver()
        {
            
            this.QueueEndpoint.Subscription = new SubscriptionConfiguration() { QueueName = "testqueue" };
            SetupModelBasicGet(this.QueueEndpoint, new BasicGetResult(1, false, "", "", 1, BasicProperties, MessageData));
            var receiverStub = new ReceiverStub();
            _receiver.AddReceiver(receiverStub);

            var result = _receiver.CheckForMessage("testendpoint");
            Assert.IsTrue(result);
            Assert.AreEqual(MessageData, receiverStub.Message.Data);
        }

        [TestMethod]
        public void ShouldCallErrorHandlerIfReceiverThrowsException()
        {
            SetupReceiver();
            this.QueueEndpoint.Subscription = new SubscriptionConfiguration()
                                                  {
                                                      QueueName = "testqueue",
                                                      ErrorHandling = new ErrorHandlingConfiguration()
                                                                          {
                                                                              RethrowExceptions = false
                                                                          }
                                                  };
            SetupModelBasicGet(this.QueueEndpoint, new BasicGetResult(1, false, "", "", 1, BasicProperties, MessageData));
            var receiver = new ReceiverThatThrowsException();
            _receiver.AddReceiver(receiver);
            _receiver.CheckForMessage("testendpoint");
            ErrorHandler.AssertWasCalled(x => x.HandleError(Arg<ReceivedMessage>.Is.Anything, Arg<string>.Is.Equal("testendpoint")));
        }

        [TestMethod]
        public void ShouldCallCloseOnConsumer()
        {

            this.QueueEndpoint.Subscription = new SubscriptionConfiguration() { QueueName = "testqueue" };
            SetupModelBasicGet(this.QueueEndpoint, new BasicGetResult(1, false, "", "", 1, BasicProperties, MessageData));
            var receiverStub = new ReceiverStub();
            _receiver.AddReceiver(receiverStub);

            var result = _receiver.CheckForMessage("testendpoint");
            ConsumerWrapper.AssertWasCalled(x => x.Close(), opt => opt.Repeat.Once());
        }
    }
}
