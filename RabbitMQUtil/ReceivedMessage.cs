using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public class ReceivedMessage : EventArgs
    {
        public ReceivedMessage(byte[] data, string exchange, MessageProperties properties, string routingKey = "")
        {
            Data = data;
            Exchange = exchange;
            Properties = properties;
            RoutingKey = routingKey;
            SuppressAck = false;
        }

        public MessageProperties Properties { get; private set; }

        public string Exchange { get; private set; }

        public string RoutingKey { get; private set; }

        public byte[] Data { get; private set; }

        public bool SuppressAck { get; set; }
    }
}
