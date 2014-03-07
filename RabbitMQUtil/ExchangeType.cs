using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public enum ExchangeType
    {
        Undefined,
        Fanout,
        Direct,
        Topic,
        Headers
    }
}
