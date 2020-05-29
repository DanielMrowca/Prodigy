using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public interface IRabbitQueuePrefixProvider
    {
        string Prefix { get; }
    }
}
