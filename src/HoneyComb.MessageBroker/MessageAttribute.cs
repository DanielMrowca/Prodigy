using System;
using System.Diagnostics.CodeAnalysis;

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
        public bool? MultiThread { get;  }

        /// <summary>
        ///     AutoAck set to TRUE improve performance --> Messages are not queued
        ///     <para><b>IMPORTANT! When AutoAck=true, then RequeueOnError is NOT WORKING! You lose message delivery guarantee!</b></para>
        /// </summary>
        public bool? AutoAck { get; }

        /// <summary>
        ///     RequeueOnError set to TRUE causes requeue specified message on error
        /// </summary>
        public bool? RequeueOnError { get;  }


        /// <summary>
        ///     Message attribute for message broker
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="queue">Queue name</param>
        /// <param name="external"></param>
        /// <param name="exchangeType"></param>
        /// <param name="queuePrefix"></param>
        /// <param name="multiThread">Define if current queue/channel should be handled parallel</param>
        /// <param name="autoAck">AutoAck set to TRUE improve performance --> Messages are not queued</param>
        /// <param name="ackOnError">RequeueOnError set to TRUE causes requeue specified message on error</param>
        public MessageAttribute(string exchange = null, string routingKey = null, string queue = null,
            bool external = false, string exchangeType = "topic", string queuePrefix = null, 
            TriState multiThread = TriState.None, TriState autoAck = TriState.None,
            TriState ackOnError = TriState.None)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Queue = queue;
            External = external;
            ExchangeType = exchangeType;
            QueuePrefix = queuePrefix;

            // We can't use bool? because is a struct and struct is not allowed to be as parameter to an attribute
            MultiThread = multiThread.ToBool();
            AutoAck = autoAck.ToBool();
            RequeueOnError = ackOnError.ToBool();
        }
    }
}
