using System;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
{
    public class Convention : IConvention
    {
        public Type Type { get; }
        public string RoutingKey { get; }
        public string Exchange { get; }
        public string Queue { get; }
        public string QueuePrefix { get; }

        public Convention(Type type, string routingKey, string exchange, string queue, string queuePrefix)
        {
            Type = type;
            RoutingKey = routingKey;
            Exchange = exchange;
            Queue = queue;
            QueuePrefix = queuePrefix;
        }
    }
}
