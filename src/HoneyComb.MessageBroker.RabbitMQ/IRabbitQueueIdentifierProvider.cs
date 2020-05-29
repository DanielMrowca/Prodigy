using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public interface IRabbitQueueIdentifierProvider
    {
        string Identifier { get; }
    }
}
