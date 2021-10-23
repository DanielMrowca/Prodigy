using System;

namespace Prodigy.MessageBroker.RabbitMQ
{
    public interface IConventionProvider
    {
        IConvention Get<T>();
        IConvention Get(Type type);
    }
}
