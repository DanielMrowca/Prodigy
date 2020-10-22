using System;
using System.Reflection;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
{
    public class RabbitQueueIdentifierProvider : IRabbitQueuePrefixProvider
    {
        public string Prefix { get; private set; }
        public RabbitQueueIdentifierProvider(string prefix = null)
        {
            Prefix = string.IsNullOrWhiteSpace(prefix) ? Assembly.GetEntryAssembly().GetName().Name : prefix;
        }
    }
}
