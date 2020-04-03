using HoneyComb.Types;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using RabbitMQ.Client;

namespace HoneyComb.MessageBroker.RabbitMQ.Initializers
{
    /// <summary>
    ///     Initialize declared exchanges by <see cref="MessageAttribute"/>
    /// </summary>
    public class RabbitMqExchangeInitializer : IInitializer
    {
        private const string DefaultExchangeType = ExchangeType.Topic;
        private readonly IConnection _connection;
        private readonly RabbitMqOptions _options;

        public RabbitMqExchangeInitializer(IConnection connection, RabbitMqOptions options)
        {
            _connection = connection;
            _options = options;
        }

        public Task InitializeAsync()
        {
            var exchanges = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(t => t.IsDefined(typeof(MessageAttribute), false))
                .Select(t => t.GetCustomAttribute<MessageAttribute>().Exchange)
                .Distinct()
                .ToList();

            using (var channel = _connection.CreateModel())
            {
                //Declare exchange from settings
                if (_options.Exchange?.Declare == true && !string.IsNullOrWhiteSpace(_options.Exchange?.Name))
                {
                    channel.ExchangeDeclare(_options.Exchange.Name, _options.Exchange.Type, _options.Exchange.Durable,
                        _options.Exchange.AutoDelete);
                }

                //Declaring exchanges depend on MessageAttribute
                foreach (var exchange in exchanges)
                {
                    if (exchange.Equals(_options.Exchange?.Name, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    channel.ExchangeDeclare(exchange, DefaultExchangeType, true);
                }

                channel.Close();
            }

            return Task.CompletedTask;
        }
    }
}
