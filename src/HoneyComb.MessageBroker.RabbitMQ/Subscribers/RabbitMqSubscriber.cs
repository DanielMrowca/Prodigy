using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Open.Serialization.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.MessageBroker.RabbitMQ.Subscribers
{
    public class RabbitMqSubscriber : IBusSubscriber
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBusPublisher _busPublisher;
        private readonly IConventionProvider _conventionProvider;
        private readonly IModel _channel;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;
        private readonly RabbitMqOptions _options;
        private readonly RabbitMqOptions.QosOptions _qosOptions;

        public RabbitMqSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _busPublisher = serviceProvider.GetRequiredService<IBusPublisher>();
            _conventionProvider = serviceProvider.GetRequiredService<IConventionProvider>();
            _channel = serviceProvider.GetRequiredService<IConnection>().CreateModel();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();
            _logger = serviceProvider.GetService<ILogger<RabbitMqSubscriber>>();
            _options = serviceProvider.GetRequiredService<RabbitMqOptions>();
            _qosOptions = _options?.Qos ?? new RabbitMqOptions.QosOptions();
            if (_qosOptions.PrefetchCount < 1)
            {
                _qosOptions.PrefetchCount = 1;
            }
        }

        public IBusSubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handle) where T : class
        {
            var convention = _conventionProvider.Get<T>();
            var durable = _options.Queue?.Durable ?? true;
            var exclusive = _options.Queue?.Exclusive ?? false;
            var autoDelete = _options.Queue?.AutoDelete ?? false;

            _channel.QueueDeclare(convention.Queue, durable, exclusive, autoDelete);
            _channel.QueueBind(convention.Queue, convention.Exchange, convention.RoutingKey);
            _channel.BasicQos(_qosOptions.PrefetchSize, _qosOptions.PrefetchCount, _qosOptions.Global);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) => await ReceivedMessage<T>(sender, args, handle);
            _channel.BasicConsume(convention.Queue, false, consumer);
            return this;

        }

        private async Task ReceivedMessage<T>(object sender, BasicDeliverEventArgs args, Func<IServiceProvider, T, object, Task> handle)
        {
            try
            {
                var messageId = args.BasicProperties.MessageId;
                var correlationId = args.BasicProperties.CorrelationId;
                var timestamp = args.BasicProperties.Timestamp.UnixTime;

                var payload = Encoding.UTF8.GetString(args.Body);
                var message = _jsonSerializer.Deserialize<T>(payload);
                await handle?.Invoke(_serviceProvider, message, null);
                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _channel.BasicAck(args.DeliveryTag, false);
                throw;
            }
        }

    }
}
