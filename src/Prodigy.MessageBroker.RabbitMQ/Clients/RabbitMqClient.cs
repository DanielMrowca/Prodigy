using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Open.Serialization.Json;
using RabbitMQ.Client;
using IConnectionFactory = Prodigy.MessageBroker.RabbitMQ.Factories.IConnectionFactory;

namespace Prodigy.MessageBroker.RabbitMQ.Clients
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly object _lock = new object();
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ConcurrentDictionary<int, IModel> _channels = new ConcurrentDictionary<int, IModel>();
        private int _channelsCount;
        private int _maxChannels;

        public RabbitMqClient(IConnectionFactory connectionFactory, RabbitMqOptions options, ILogger<RabbitMqClient> logger, IJsonSerializer jsonSerializer)
        {
            _connectionFactory = connectionFactory;
            _options = options;
            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _maxChannels = options.MaxProducerChannels <= 0 ? 1000 : options.MaxProducerChannels;
        }

        public void Send(object message, IConvention convention, string messageId = null,
            string correlationId = null, string spanContext = null, object messageContext = null, IDictionary<string, object> headers = null)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_channels.TryGetValue(threadId, out var channel))
            {
                lock (_lock)
                {
                    if (_channelsCount >= _maxChannels)
                    {
                        throw new InvalidOperationException($"Cannot create RabbitMQ producer channel for thread: {threadId} " +
                                                            $"(reached the limit of {_maxChannels} channels). " +
                                                            "Modify `MaxProducerChannels` setting to allow more channels.");
                    }

                    channel = _connectionFactory.GetConnection().CreateModel();
                    _channels.TryAdd(threadId, channel);
                    _logger.LogTrace("Created a channel for thread: {ThreadId}, total channels: {ChannelsCount}/{MaxChannels}", threadId, _channelsCount, _maxChannels);
                    _channelsCount++;
                }
            }
            else
            {
                _logger.LogTrace("Reused a channel for thread: {ThreadId}, total channels: {ChannelsCount}/{MaxChannels}", threadId, _channelsCount, _maxChannels);
            }


            var json = _jsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = GetProperties(channel,messageId, correlationId, spanContext, headers);
            _logger.LogTrace("Publishing MessageId: {MessageId}, CorrelationId: {CorrelationId}, {@Message}", properties.MessageId, properties.CorrelationId, json);
            channel.BasicPublish(convention.Exchange, convention.RoutingKey, properties, body);
            
            
        }

        private IBasicProperties GetProperties(IModel channel, string messageId = null, string correlationId = null, string spanContext = null, IDictionary<string, object> headers = null)
        {
            var properties = channel.CreateBasicProperties();
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
