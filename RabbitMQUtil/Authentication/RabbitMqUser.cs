namespace RabbitMQUtil.Authentication
{
    public class RabbitMqUser
    {
        public RabbitMqUser(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(UserName); }
        }
    }
}
