using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RabbitMQUtil.Tests
{
    [TestClass]
    public class MessagePropertiesTests
    {
        [TestMethod]
        public void GetEncodingShouldReturnUtf8ByDefault()
        {
            var props = new MessageProperties()
                            {
                                ContentEncoding = string.Empty
                            };
            Assert.IsInstanceOfType(props.GetEncoding(), typeof (UTF8Encoding));
        }

        [TestMethod]
        public void GetEncodingShouldReturnUtf7IfSet()
        {
            var props = new MessageProperties()
            {
                ContentEncoding = "UTF-7"
            };
            Assert.IsInstanceOfType(props.GetEncoding(), typeof(UTF7Encoding));
        }
    }
}
