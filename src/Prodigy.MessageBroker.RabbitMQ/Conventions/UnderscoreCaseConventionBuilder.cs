using System;
using System.Linq;
using System.Reflection;

namespace Prodigy.MessageBroker.RabbitMQ.Conventions
{
    /// <summary>
    ///     Create exchange, queue and routing key names using underscore convention.
    ///     RabbitMqConvention: <see href="https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html"/>
    /// </summary>
    public class UnderscoreCaseConventionBuilder : IConventionBuilder
    {
        private readonly RabbitMqOptions _options;
        private readonly IRabbitQueuePrefixProvider _identificationProvider;

        public UnderscoreCaseConventionBuilder(RabbitMqOptions options, IRabbitQueuePrefixProvider identificationProvider)
        {
            _options = options;
            _identificationProvider = identificationProvider;
        }

        public string GetExchange(Type type)
        {
            var attribute = GetAttribute(type);
            var exchange = type.Assembly.GetName().Name;

            if (!string.IsNullOrWhiteSpace(attribute?.Exchange))
                exchange = attribute.Exchange;
            else if (!string.IsNullOrWhiteSpace(_options.Exchange?.Name))
                exchange = _options.Exchange.Name;

            return ToUnderscoreCase(exchange);
        }

        public string GetQueue(Type type)
        {
            string queue;
            var attribute = GetAttribute(type);
            if (!string.IsNullOrWhiteSpace(attribute?.Queue))
                queue = attribute.Queue;
            else
                queue = $"{GetQueuePrefix(type)}/{GetExchange(type)}.{type.Name}";

            return ToUnderscoreCase(queue);
        }

        public string GetRoutingKey(Type type)
        {
            var attribute = GetAttribute(type);
            var routingKey = type.Name;
            if (!string.IsNullOrWhiteSpace(attribute?.RoutingKey))
                routingKey = attribute.RoutingKey;

            return ToUnderscoreCase(routingKey);

        }

        public string GetQueuePrefix(Type type)
        {
            var attribute = GetAttribute(type);
            return string.IsNullOrWhiteSpace(attribute?.QueuePrefix) ? _identificationProvider.Prefix : attribute.QueuePrefix;
        }

        public bool GetMultiThread(Type type)
        {
            var attribute = GetAttribute(type);
            return attribute?.MultiThread ?? _options.MultiThread;
        }

        public bool? GetAutoAck(Type type)
        {
            var attribute = GetAttribute(type);
            return attribute?.AutoAck ?? _options.AutoAck;
        }

        public bool? GetAckOnError(Type type)
        {
            var attribute = GetAttribute(type);
            return attribute?.RequeueOnError ?? _options.RequeueOnError;
        }

        private static string ToUnderscoreCase(string str)
            => string.Concat(str.Select((x, i) => i > 0 && str[i - 1] != '.' && str[i - 1] != '/' && char.IsUpper(x) ?
            "_" + x :
            x.ToString())).ToLower();


        private static MessageAttribute GetAttribute(MemberInfo type) => type.GetCustomAttribute<MessageAttribute>();


    }
}
