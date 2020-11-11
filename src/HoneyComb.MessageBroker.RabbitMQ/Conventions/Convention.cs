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
        public bool MultiThread { get; }
        public bool? AutoAck { get; }
        public bool? AckOnError { get; }

        public Convention(
            Type type, 
            string routingKey, 
            string exchange, 
            string queue, 
            string queuePrefix, 
            bool multiThread = false, 
            bool? autoAck = null, 
            bool? ackOnError = null)
        {
            Type = type;
            RoutingKey = routingKey;
            Exchange = exchange;
            Queue = queue;
            QueuePrefix = queuePrefix;
            MultiThread = multiThread;
            AutoAck = autoAck;
            AckOnError = ackOnError;
        }
    }
}
