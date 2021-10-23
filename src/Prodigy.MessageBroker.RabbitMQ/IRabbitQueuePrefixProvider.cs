namespace Prodigy.MessageBroker.RabbitMQ
{
    public interface IRabbitQueuePrefixProvider
    {
        string Prefix { get; }
    }
}
