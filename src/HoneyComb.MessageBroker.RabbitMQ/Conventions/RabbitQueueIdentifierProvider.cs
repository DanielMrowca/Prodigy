using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
{
    public class RabbitQueueIdentifierProvider : IRabbitQueueIdentifierProvider
    {
        public string Identifier { get; private set; }

        public RabbitQueueIdentifierProvider()
        {
            Identifier = Guid.NewGuid().ToString();
        }
    }
}
