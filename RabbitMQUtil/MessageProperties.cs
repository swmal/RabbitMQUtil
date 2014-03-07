using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQUtil
{
    public class MessageProperties
    {
        public MessageProperties()
        {
            Headers = new Dictionary<string, string>();
        }

        public static MessageProperties Default
        {
            get
            {
                return new MessageProperties()
                           {
                               MessageId = Guid.NewGuid().ToString(),
                               DeliveryMode = DeliveryMode.Persistent
                           };
            }
        }

        public Encoding GetEncoding()
        {
            var defaultEncoding = Encoding.UTF8;
            try
            {
                return Encoding.GetEncoding(ContentEncoding ?? "UTF-8");
            }
            catch{}
            return defaultEncoding;   
        }

        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public DeliveryMode DeliveryMode { get; set; }
        public IDictionary<string, string> Headers { get; private set; }
    }
}
