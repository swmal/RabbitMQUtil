using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public interface IReceiver
    {
        void Receive(ReceivedMessage message);
    }
}
