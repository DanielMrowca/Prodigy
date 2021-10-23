using System;

namespace Prodigy.MessageBroker
{
    public interface IConvention
    {
        Type Type { get; }
        string RoutingKey { get; }
        string Exchange { get; }
        string Queue { get; }
        string QueuePrefix { get; }

        /// <summary>
        ///     Define if current queue/channel should be handled parallel
        /// </summary>
        bool MultiThread { get; }

        /// <summary>
        ///     AutoAck set to TRUE improve performance --> Messages are not queued
        ///     <para><b>IMPORTANT! When AutoAck=true, then RequeueOnError is NOT WORKING! You lose message delivery guarantee!</b></para>
        /// </summary>
        bool? AutoAck { get; }

        /// <summary>
        ///     RequeueOnError set to TRUE causes requeue specified message on error
        ///     <para><para><b>IMPORTANT! It's working only when AutoAck=false</b></para></para>
        /// </summary>
        bool? RequeueError { get; }

    }
}
