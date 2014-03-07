using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Authentication
{
    public class RabbitMqUserProvider
    {
        private readonly AppSettingsWrapper _appSettings;

        public RabbitMqUserProvider()
            : this(new AppSettingsWrapper())
        {
            
        }

        public RabbitMqUserProvider(AppSettingsWrapper appSettings)
        {
            _appSettings = appSettings;
        }

        public virtual RabbitMqUser GetUser()
        {
            var userName = _appSettings.GetSetting("RabbitMq.Username");
            var password = _appSettings.GetSetting("RabbitMq.Password");
            return new RabbitMqUser(userName, password);
        }
    }
}
