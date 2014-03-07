using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQUtil.Configuration;
using RabbitMQ.Client;
using RabbitMQUtil.Logging;

namespace RabbitMQUtil
{
    public class ChannelConfigurator
    {
        private readonly RabbitMqLogger _logger;

        public ChannelConfigurator()
            : this(new RabbitMqLogger())
        {
            
        }

        public ChannelConfigurator(RabbitMqLogger logger)
        {
            _logger = logger;
        }

        public virtual void ConfigureQueue(QueueEndpoint endpoint, IModel channel)
        {
            HandleExchange(endpoint, channel);
            if (endpoint.PubSubType != PubSubType.Subscribe) return;
            HandleSubscription(endpoint, channel);
        }

        public virtual void ConfigureErrorHandling(QueueEndpoint endpoint, IModel channel)
        {
            HandleErrorConfig(endpoint.Subscription, channel);
        }

        private static void HandleErrorConfig(SubscriptionConfiguration subscriptionConfig, IModel channel)
        {
            if (
                subscriptionConfig.ErrorHandling == null 
                || 
                !subscriptionConfig.ErrorHandling.EnableErrorQueue 
                || 
                string.IsNullOrEmpty(subscriptionConfig.ErrorHandling.ErrorQueueName)) return;

            channel.QueueDeclare(subscriptionConfig.ErrorHandling.ErrorQueueName, true, false,
                                                       false, null);
        }

        private static void HandleSubscription(QueueEndpoint endpoint, IModel channel)
        {
            if (endpoint.Subscription == null)
            {
                throw new InvalidOperationException(
                    "No subscription configuration was supplied. When an endpoints PubSubType is 'Subscribe', subscription configuration is mandatory.");
            }
            var queueName = string.IsNullOrWhiteSpace(endpoint.Subscription.QueueName)
                                ? channel.QueueDeclare()
                                : channel.QueueDeclare(endpoint.Subscription.QueueName, endpoint.Subscription.Durable, false,
                                                       false, null);
            for (var x = 0; x < endpoint.Subscription.ExchangeBindings.Count; x++)
            {
                var exchangeBinding = endpoint.Subscription.ExchangeBindings[x];
                if (exchangeBinding.DeclareExchange)
                {
                    channel.ExchangeDeclare(exchangeBinding.Name, exchangeBinding.Type.ToString().ToLower());
                }
                channel.QueueBind(queueName, exchangeBinding.Name, exchangeBinding.RoutingKey);
            }
        }

        private void HandleExchange(QueueEndpoint endpoint, IModel channel)
        {
            if (
                endpoint.Exchange != null
                &&
                !string.IsNullOrEmpty(endpoint.Exchange.Name))
            {
                LogExchangeDeclareData(endpoint);
                channel.ExchangeDeclare(endpoint.Exchange.Name, endpoint.Exchange.Type.ToString().ToLower(),
                                        endpoint.Exchange.Durable);
            }
        }

        private void LogExchangeDeclareData(QueueEndpoint endpoint)
        {
            _logger.Log("Declaring exchange: " + endpoint.Exchange.Name);
            _logger.Log("Exchange type: " + endpoint.Exchange.Type.ToString());
            _logger.Log("Exchange durable: " + endpoint.Exchange.Durable.ToString());
        }
    }
}
