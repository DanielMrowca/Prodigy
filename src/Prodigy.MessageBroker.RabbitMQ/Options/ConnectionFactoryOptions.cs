namespace Prodigy.MessageBroker.RabbitMQ.Options
{
    public class ConnectionFactoryOptions
    {
        public bool ConnectionRetryForever { get; set; } = true;
        public int ConnectionRetryCount { get; set; } = 10;
    }
}
