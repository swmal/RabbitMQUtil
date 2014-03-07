using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RabbitMQUtil.Authentication
{
    public class AppSettingsWrapper
    {
        public virtual string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
