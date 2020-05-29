using HoneyComb.Types;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace HoneyComb.MessageBroker.RabbitMQ.Initializers
{
    /// <summary>
    ///     Initialize declared exchanges by <see cref="MessageAttribute"/> and <see cref="RabbitMqOptions.Exchange"/>
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
            if ((_options.Exchange is null || string.IsNullOrWhiteSpace(_options.Exchange.Name) && _options.Exchange?.Declare == true))
                throw new InvalidOperationException("RabbitMq exchange name must be set in RabbitMqOptions.Exchange.Name. " +
                    "Add option in AddRabbitMQ(..) method or in appsettings.json");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var messageAttributes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsDefined(typeof(MessageAttribute), false))
                .Select(t => t.GetCustomAttribute<MessageAttribute>())
                .Distinct()
                .ToList();

            using (var channel = _connection.CreateModel())
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
