using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rhino.Mocks;
using RabbitMQUtil.Configuration;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class ReceiveListenerTests : ReceiverTestsBase
    {
        private ReceiveListener _listener;

        [TestInitialize]
        public void Setup()
        {
            InternalSetup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _listener = null;
        }

        private void SetupListener(CorrelationIdState correlationIdState, bool rethrowExceptions = false, string routingKey = "")
        {
            BasicProperties = MockRepository.GenerateStub<IBasicProperties>();
            CreateConsumerMock(correlationIdState, routingKey);
            SetupQueueEndpointProvider("testendpoint", "vhost", "testqueue");
            SetupRabbitMqFactory();
            SetupErrorHandler(rethrowExceptions);
            _listener = new ReceiveListener(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator, ErrorHandler);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SendThrowIfStartedWithoutReceivers()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            _listener = new ReceiveListener(QueueEndpointProvider, RabbitMqFactory, ChannelConfigurator, ErrorHandler);
            _listener.Start("testendpoint");
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void StartShouldThrowIfPubSubTypeIsPublish()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            QueueEndpoint.PubSubType = PubSubType.Publish;
            _listener.Start("testendpoint");
        }

        [TestMethod]
        public void StartShouldSetMessageIdOnEvent()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual(MessageId, receiver.Message.Properties.MessageId);
        }

        [TestMethod]
        public void StartShouldCallChannelConfigurator()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            ChannelConfigurator.AssertWasCalled(x => x.ConfigureQueue(QueueEndpoint, Model));
        }

        [TestMethod]
        public void StartShouldSetCorrelationIdOnEvent()
        {
            SetupListener(CorrelationIdState.Use);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual(CorrelationId, receiver.Message.Properties.CorrelationId);
        }

        [TestMethod]
        public void StartShouldSetContentTypeOnEvent()
        {
            SetupListener(CorrelationIdState.Use);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual(ContentType, receiver.Message.Properties.ContentType);
        }

        [TestMethod]
        public void StartShouldSetContentEncodingOnEvent()
        {
            SetupListener(CorrelationIdState.Use);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual(ContentEncoding, receiver.Message.Properties.ContentEncoding);
        }

        [TestMethod]
        public void StartShouldSetMessageDataOnEvent()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual(MessageData, receiver.Message.Data);
        }

        [TestMethod]
        public void ShouldSetExchangeOnReceivedMessage()
        {
            SetupListener(CorrelationIdState.DoNotUse);
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual("ex1", receiver.Message.Exchange);
        }

        [TestMethod]
        public void ShouldSetRoutingKeyOnReceivedMessage()
        {
            SetupListener(CorrelationIdState.DoNotUse, false, "myRoutingKey");
            var receiver = new ReceiverStub(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            Assert.AreEqual("myRoutingKey", receiver.Message.RoutingKey);
        }

        [TestMethod]
        public void ShouldCallErrorHandlerIfReceiverThrowsException()
        {
           
            SetupListener(CorrelationIdState.DoNotUse);
           
            var receiver = new ReceiverThatThrowsException(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
            ErrorHandler.AssertWasCalled(x => x.HandleError(Arg<ReceivedMessage>.Is.Anything, Arg<string>.Is.Equal("testendpoint")));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ShouldRethrowIfReceiverThrowsExceptionAndRethrowIsConfigured()
        {
            SetupListener(CorrelationIdState.DoNotUse, true);
            this.QueueEndpoint.Subscription = new SubscriptionConfiguration()
            {
                QueueName = "testqueue",
                ErrorHandling = new ErrorHandlingConfiguration()
                {
                    RethrowExceptions = true
                }
            };
            var receiver = new ReceiverThatThrowsException(_listener);
            _listener.AddReceiver(receiver);
            _listener.Start("testendpoint");
        }
    }
}
