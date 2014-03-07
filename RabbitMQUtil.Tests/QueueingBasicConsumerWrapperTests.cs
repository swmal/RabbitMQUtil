using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Util;
using Rhino.Mocks;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class When_closing_QueueingBasicConsumerWrapper
    {
        private IConnection _connection;
        private QueueingBasicConsumer _basicConsumer;
        private QueueingBasicConsumerWrapper _consumerWrapper;
        private IModel _channel;

        [TestInitialize]
        public void Setup()
        {
            _connection = MockRepository.GenerateMock<IConnection>();
            _channel = MockRepository.GenerateMock<IModel>();
            _basicConsumer = MockRepository.GenerateStub<QueueingBasicConsumer>();
            
            _consumerWrapper = new QueueingBasicConsumerWrapper(_basicConsumer, _connection, _channel);
        }

        [TestMethod]
        public void ItShouldCloseTheChannelIfTheChannelIsOpen()
        {
            _channel.Stub(x => x.IsOpen).Return(true);
            
            _consumerWrapper.Close();

            _channel.AssertWasCalled(x => x.Close());
        }

        [TestMethod]
        public void ItShouldNotTryToCloseTheChannelIfItsAlreadyClosed()
        {
            _channel.Stub(x => x.IsOpen).Return(false);

            _consumerWrapper.Close();

            _channel.AssertWasNotCalled(x => x.Close());
        }

        [TestMethod]
        public void ItShouldCloseConnectionIfConnectionIsOpen()
        {
            _connection.Stub(x => x.IsOpen).Return(true);
            
            _consumerWrapper.Close();

            _connection.AssertWasCalled(x => x.Close());
        }

        [TestMethod]
        public void ItShouldNotTryToCloseTheConnectionIfItsAlreadyClosed()
        {
            _connection.Stub(x => x.IsOpen).Return(false);

            _consumerWrapper.Close();

            _connection.AssertWasNotCalled(x => x.Close());
        }

        [TestMethod]
        public void ItShouldDisposeConnection()
        {
            _consumerWrapper.Close();
            _connection.AssertWasCalled(x => x.Dispose(), opt => opt.Repeat.Once());
        }
    }
}
