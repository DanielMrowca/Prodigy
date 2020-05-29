using System;
using System.Collections.Generic;
using System.Text;

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
        public bool AddUniqueIdentifierToQueueName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Rputing key</param>
        /// <param name="queue">Queue name</param>
        /// <param name="external">True/False</param>
        /// <param name="queuePrefix">Queue prefix is active when queue name is empty</param>
        public MessageAttribute(string exchange = null, string routingKey = null, string queue = null,
            bool external = false, string queuePrefix = null, string exchangeType = "topic", bool addUniqueIdentifierToQueueName = false)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Queue = queue;
            External = external;
            QueuePrefix = queuePrefix;
            ExchangeType = exchangeType;
            AddUniqueIdentifierToQueueName = addUniqueIdentifierToQueueName;
        }
    }
}
