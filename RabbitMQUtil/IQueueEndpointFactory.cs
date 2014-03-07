using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public interface IQueueEndpointFactory
    {
        ISender GetSender(string name);
    }
}
