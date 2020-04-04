using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Open.Serialization.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker.RabbitMQ.Clients
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly IModel _channel;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        public RabbitMqClient(IConnection connection, RabbitMqOptions options, ILogger<RabbitMqClient> logger, IJsonSerializer jsonSerializer)
        {
            _channel = connection.CreateModel();
            _options = options;
            _logger = logger;
            _jsonSerializer = jsonSerializer;
        }

        public void Send(object message, IConvention convention, string messageId = null,
            string correlationId = null, string spanContext = null, object messageContext = null, IDictionary<string, object> headers = null)
        {
            var json = _jsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = GetProperties(messageId, correlationId, spanContext, headers);
            _channel.BasicPublish(convention.Exchange, convention.RoutingKey, properties, body);

        }

        private IBasicProperties GetProperties(string messageId = null, string correlationId = null, string spanContext = null, IDictionary<string, object> headers = null)
        {
            var properties = _channel.CreateBasicProperties();
            properties.MessageId = string.IsNullOrWhiteSpace(messageId) ?
                Guid.NewGuid().ToString("N") :
                messageId;
            properties.CorrelationId = string.IsNullOrWhiteSpace(correlationId)
                ? Guid.NewGuid().ToString("N")
                : correlationId;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Headers = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(spanContext))
                properties.Headers.Add("span_context", spanContext);

            if (headers is { })
            {
                foreach (var (key, value) in headers)
                {
                    if (string.IsNullOrWhiteSpace(key) || value is null)
                    {
                        continue;
                    }

                    properties.Headers.TryAdd(key, value);
                }
            }
            //if (messageContext is { })
            //{
            //    properties.Headers.Add(_contextProvider.HeaderName, _serializer.Serialize(context));
            //    return;
            //}

            //properties.Headers.Add(_contextProvider.HeaderName, EmptyContext);

            return properties;
        }
    }
}
