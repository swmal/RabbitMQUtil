using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Factories
{
    public class ReceiverImpl : IReceiver
    {
        private readonly Action<ReceivedMessage> _action;

        public ReceiverImpl(Action<ReceivedMessage> action)
        {
            _action = action;
        }



        public void Receive(ReceivedMessage message)
        {
            _action.Invoke(message);
        }
    }
}
