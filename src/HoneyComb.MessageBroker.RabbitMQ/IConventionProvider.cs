using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public interface IConventionProvider
    {
        IConvention Get<T>();
        IConvention Get(Type type);
    }
}
