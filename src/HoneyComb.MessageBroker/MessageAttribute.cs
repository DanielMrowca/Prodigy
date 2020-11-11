using System;

namespace HoneyComb.MessageBroker
{
    public sealed class MessageAttribute : Attribute
    {
        public string Exchange { get; }
        public string ExchangeType { get; }
        public string RoutingKey { get; }
        public string Queue { get; }
        public bool External { get; }
        public string QueuePrefix { get; }

        /// <summary>
        ///     Define if current queue/channel should be handled parallel
        /// </summary>
        public bool MultiThread { get; }

        /// <summary>
        ///     AutoAck set to TRUE improve performance --> Messages are not queued
        /// </summary>
        public bool? AutoAck { get; }

        /// <summary>
        ///     AckOnError set to FALSE causes no acknowledgment of the message in case of error
        /// </summary>
        public bool? AckOnError { get; }

        /// <summary>
        ///     Message attribute for message broker
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="queue">Queue name</param>
        /// <param name="external"></param>
        /// <param name="exchangeType"></param>
        /// <param name="queuePrefix"></param>
        /// <param name="autoAck">AutoAck set to TRUE improve performance --> Messages are not queued</param>
        public MessageAttribute(string exchange = null, string routingKey = null, string queue = null,
            bool external = false, string exchangeType = "topic", string queuePrefix = null, bool multiThread = false, bool autoAck = false, bool ackOnError = true)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Queue = queue;
            External = external;
            ExchangeType = exchangeType;
            QueuePrefix = queuePrefix;
            MultiThread = multiThread;
            AutoAck = autoAck;
            AckOnError = ackOnError;
        }
    }
}
