using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public interface IQueueEndpointProvider
    {
        QueueEndpoint GetEndpointByName(string name);
        string GetHostName();
    }
}
