using HoneyComb.Types;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using RabbitMQ.Client;

namespace HoneyComb.MessageBroker.RabbitMQ.Initializers
{
    /// <summary>
    ///     Initialize declared exchanges by <see cref="MessageAttribute"/> and <see cref="RabbitMqOptions.Exchange"/>
    /// </summary>
    public class RabbitMqExchangeInitializer : IInitializer
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMqOptions _options;

        public RabbitMqExchangeInitializer(IConnectionFactory connectionFactory, RabbitMqOptions options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
        }

        public Task InitializeAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.Exchange?.Name) && _options.Exchange?.Declare == true)
                throw new InvalidOperationException("When RabbitMqOptions.Exchange.Declare = true then exchange name must be set in RabbitMqOptions.Exchange.Name. " +
                    "Add option in AddRabbitMQ(..) method or in appsettings.json. Just set RabbitMqOptions.Exchange.Declare = false to skip this exception");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var messageAttributes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsDefined(typeof(MessageAttribute), false))
                .Select(t => t.GetCustomAttribute<MessageAttribute>())
                .Distinct()
                .ToList();

            using (var channel = _connectionFactory.GetConnection().CreateModel())
            {
                if (_options.Exchange?.Declare == true)
                {
                    channel.ExchangeDeclare(_options.Exchange.Name, _options.Exchange.Type, _options.Exchange.Durable,
                        _options.Exchange.AutoDelete);
                }

                //Declaring exchanges depend on MessageAttribute
                foreach (var attribute in messageAttributes)
                {
                    if (attribute is null || (attribute.Exchange.Equals(_options.Exchange?.Name, StringComparison.InvariantCultureIgnoreCase) && _options.Exchange?.Declare == true))
                        continue;

                    channel.ExchangeDeclare(attribute.Exchange, attribute.ExchangeType, true);
                }

                channel.Close();
            }

            return Task.CompletedTask;
        }
    }
}
