using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQUtil
{
    public class QueueingBasicConsumerWrapper
    {
        private readonly QueueingBasicConsumer _consumer;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public QueueingBasicConsumerWrapper(QueueingBasicConsumer consumer, IConnection connection, IModel channel)
        {
            _consumer = consumer;
            _connection = connection;
            _channel = channel;
        }

        public virtual QueueingBasicConsumer GetConsumer()
        {
            return _consumer;
        }

        public virtual object Dequeue()
        {
            return _consumer.Queue.Dequeue();
        }

        public virtual void Close()
        {
            if (_channel.IsOpen)
                _channel.Close();

            if (_connection.IsOpen)
                _connection.Close();

            _connection.Dispose();
        }
    }
}
