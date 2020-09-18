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

        /// <summary>
        ///     Message attribute for message broker
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Rputing key</param>
        /// <param name="queue">Queue name</param>
        /// <param name="external">True/False</param>
        public MessageAttribute(string exchange = null, string routingKey = null, string queue = null,
            bool external = false, string exchangeType = "topic")
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Queue = queue;
            External = external;
            ExchangeType = exchangeType;
        }
    }
}
