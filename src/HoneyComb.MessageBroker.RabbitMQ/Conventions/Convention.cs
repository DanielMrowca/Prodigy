using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
{
    public class Convention : IConvention
    {
        public Type Type { get; }
        public string RoutingKey { get; }
        public string Exchange { get; }
        public string Queue { get; }

        public Convention(Type type, string routingKey, string exchange, string queue)
        {
            Type = type;
            RoutingKey = routingKey;
            Exchange = exchange;
            Queue = queue;
        }
    }
}
