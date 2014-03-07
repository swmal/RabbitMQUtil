using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public interface IErrorHandler
    {
        bool ShouldRethrowExceptions(string endpointName);
        void HandleError(ReceivedMessage message, string endpointName);
    }
}
