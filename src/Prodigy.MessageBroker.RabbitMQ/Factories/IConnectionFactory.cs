using RabbitMQ.Client;

namespace Prodigy.MessageBroker.RabbitMQ.Factories
{
    public interface IConnectionFactory
    {
        IConnection GetConnection();
    }
}
