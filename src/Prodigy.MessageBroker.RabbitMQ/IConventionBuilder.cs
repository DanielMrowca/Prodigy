using System;

namespace Prodigy.MessageBroker.RabbitMQ
{
    public interface IConventionBuilder
    {
        string GetRoutingKey(Type type);
        string GetExchange(Type type);
        string GetQueue(Type type);
        string GetQueuePrefix(Type type);
        bool GetMultiThread(Type type);
        bool? GetAutoAck(Type type);
        bool? GetAckOnError(Type type);
    }
}
