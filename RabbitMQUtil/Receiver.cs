using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQUtil.Configuration;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil
{
    public abstract class Receiver
    {
        protected readonly IQueueEndpointProvider QueueEndpointProvider;
        protected readonly RabbitMqFactory RabbitMqFactory;
        protected readonly ChannelConfigurator ChannelConfigurator;
        protected readonly IErrorHandler ErrorHandler;
        protected readonly RabbitMqLogger Logger;
        protected string EndpointName;

        protected Receiver(
            IQueueEndpointProvider queueEndpointProvider, 
            RabbitMqFactory rabbitMqFactory, 
            ChannelConfigurator channelConfigurator,
            IErrorHandler errorHandler,
            RabbitMqLogger logger)
        {
            QueueEndpointProvider = queueEndpointProvider;
            RabbitMqFactory = rabbitMqFactory;
            ChannelConfigurator = channelConfigurator;
            ErrorHandler = errorHandler;
            Logger = logger;
        }

        protected readonly List<IReceiver> Receivers = new List<IReceiver>();

        public virtual void AddReceiver(IReceiver receiver)
        {
            Receivers.Add(receiver);
        }

        protected void ValidateEndpoint(QueueEndpoint endpoint)
        {
            if (endpoint.PubSubType == PubSubType.Publish) throw new InvalidOperationException("ReceiveListener cannot be started on an endpoint with PubSubType.Publish.");
        }

        private MessageProperties BuildMessageProperties(IModel channel, SubscriptionConfiguration subscription, IBasicProperties properties)
        {
            var messageProperties = new MessageProperties
            {
                MessageId = properties.MessageId,
                CorrelationId = properties.CorrelationId,
                ContentType = properties.ContentType,
                ContentEncoding = properties.ContentEncoding
            };
            if (properties.Headers != null && properties.Headers.Keys.Count > 0)
            {
                foreach (var key in properties.Headers.Keys)
                {
                    try
                    {
                        var val = GetStringFromByteArray(properties.Headers[key], properties.ContentEncoding) ?? string.Empty;
                        messageProperties.Headers.Add(key.ToString(), val);                    
                    }
                    catch { }
                    
                }
            }
            return messageProperties;
        }

        private void AckMessage(IModel channel, ulong deliveryTag, bool noAck, ReceivedMessage message)
        {
            if (!noAck && message != null && !message.SuppressAck)
            {
                channel.BasicAck(deliveryTag, false);
            }
        }

        protected void HandleReceivedMessage(IModel channel, SubscriptionConfiguration subscription, BasicGetResult result)
        {
            var messageProperties = BuildMessageProperties(channel, subscription, result.BasicProperties);
            
            ReceivedMessage message = null;
            try
            {
                OnMessageReceived(result.Body, result.Exchange, result.RoutingKey, messageProperties, out message);
            }
            catch (Exception e)
            {
                Logger.Log("Exception occureed while receiving message");
                Logger.Log("Message: " + e.Message);
                Logger.Log("Stacktrace: " + e.StackTrace);
                ErrorHandler.HandleError(message, EndpointName);
                if (ErrorHandler.ShouldRethrowExceptions(EndpointName)) throw;
            }
            finally
            {
                AckMessage(channel, result.DeliveryTag, subscription.NoAck, message);
            }   
        }


        protected void HandleReceivedMessage(IModel channel, SubscriptionConfiguration subscription, BasicDeliverEventArgs ea)
        {
            var messageProperties = BuildMessageProperties(channel, subscription, ea.BasicProperties);
            LogReceivedMessage(ea.Body, ea.Exchange, messageProperties);
            ReceivedMessage message = null;
            try
            {
                OnMessageReceived(ea.Body, ea.Exchange, ea.RoutingKey, messageProperties, out message);
            }
            catch (Exception e)
            {
                Logger.Log("Exception occureed while receiving message");
                Logger.Log("Message: " + e.Message);
                Logger.Log("Stacktrace: " + e.StackTrace);
                ErrorHandler.HandleError(message, EndpointName);
                if (ErrorHandler.ShouldRethrowExceptions(EndpointName)) throw;
            }
            finally
            {
                AckMessage(channel, ea.DeliveryTag, subscription.NoAck, message);
            }   
        }

        private void OnMessageReceived(byte[] data, string exchange, string routingKey, MessageProperties properties, out ReceivedMessage message)
        {
            var receivedMessage = new ReceivedMessage(data, exchange, properties, routingKey);
            message = receivedMessage;
            Receivers.ForEach(x => x.Receive(receivedMessage));
        }

        protected string GetStringFromByteArray(object bytes, string encoding)
        {
            var byteArr = bytes as byte[];
            if (byteArr == null) throw new ArgumentException("no valid byte array was supplied", "bytes");
            Encoding enc;
            try
            {
                enc = Encoding.GetEncoding(encoding);
            }
            catch
            {

                enc = Encoding.UTF8;
            }
            return enc.GetString(byteArr);
        }

        private void LogReceivedMessage(byte[] data, string exchange, MessageProperties properties)
        {
            Logger.Log("Message received");
            Logger.Log("Message.Exchange=" + exchange);
            Logger.Log("Message.Size(bytes)=" + (data != null ? data.Length.ToString(CultureInfo.InvariantCulture) : "0"));
            Logger.Log("Message.Id" + properties.MessageId);
            foreach (var header in properties.Headers)
            {
                Logger.Log(string.Format("Header: '{0}': '{1}'", header.Key, header.Value));
            }
        }
    }
}
