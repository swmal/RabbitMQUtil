namespace RabbitMQUtil.Logging
{
    public class RabbitMqLogger
    {
        private IRabbitMqLogger _logger;
        private static RabbitMqLogger _nullLogger = new RabbitMqLogger();

        public static RabbitMqLogger NullLogger
        {
            get { return _nullLogger; }
        }

        public virtual void SetLogger(IRabbitMqLogger logger)
        {
            _logger = logger;
        }

        public virtual void Log(string message)
        {
            if (_logger != null)
            {
                _logger.Debug(message);
            }
        }
    }
}
