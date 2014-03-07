using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public interface ISender
    {
        void Send(string endpointName, string data, MessageProperties properties);
        void Send(string endpointName, byte[] data, MessageProperties properties);
        void Send(string endpointName, byte[] data, MessageProperties properties, string routingKey);
    }
}
