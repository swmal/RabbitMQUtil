using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQUtil
{
    public interface IReceiveListener : IDisposable
    {
        void Start(string endpoint);
        Thread StartInThread(string endpoint);
        void Stop();
        void AddReceiver(IReceiver receiver);
    }
}
