using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Configuration
{
    public class ExchangeBindingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExchangeBinding();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var binding = (ExchangeBinding) element;
            return binding.Name + binding.RoutingKey;
        }

        public ExchangeBinding this[int index]
        {
            get { return BaseGet(index) as ExchangeBinding; }
        }

        public void Add(ExchangeBinding binding)
        {
            BaseAdd(binding);
        }
    }
}
