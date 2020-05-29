using System;
using System.Linq;
using System.Reflection;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
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
            {
                var prefix = _identificationProvider.Prefix;
                queue = $"{prefix}/{GetExchange(type)}.{type.Name}";
            }

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

        private static string ToUnderscoreCase(string str)
            => string.Concat(str.Select((x, i) => i > 0 && str[i - 1] != '.' && str[i - 1] != '/' && char.IsUpper(x) ?
            "_" + x :
            x.ToString())).ToLower();


        private static MessageAttribute GetAttribute(MemberInfo type) => type.GetCustomAttribute<MessageAttribute>();
    }
}
