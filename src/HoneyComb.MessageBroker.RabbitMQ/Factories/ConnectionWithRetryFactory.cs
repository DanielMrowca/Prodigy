using HoneyComb.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace HoneyComb.MessageBroker.RabbitMQ.Factories
{
    public class ConnectionWithRetryFactory : IConnectionFactory
    {
        private static readonly object _lock = new object();
        private static IConnection _connection;

        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMqOptions _options;
        private readonly IHoneyCombBuilder _honeyCombBuilder;


        public ConnectionWithRetryFactory(ConnectionFactory connectionFactory,
            RabbitMqOptions options, IHoneyCombBuilder honeyCombBuilder)
        {
            _connectionFactory = connectionFactory;
            _options = options;
            _honeyCombBuilder = honeyCombBuilder;
        }

        public IConnection GetConnection()
        {
            lock (_lock)
            {
                if (_connection != null )
                    return _connection;

                var logger = _honeyCombBuilder.Services.BuildServiceProvider().GetRequiredService<ILogger<ConnectionWithRetryFactory>>();
                var policyBuilder = Policy.Handle<Exception>();
                RetryPolicy retryPolicy = null;


                if (_options.ConnectionRetryForever || (!_options.ConnectionRetryForever && _options.ConnectionRetryCount < 0))
                    retryPolicy = policyBuilder.WaitAndRetryForever((r, e, ctx) => TimeSpan.FromSeconds(3 * r), OnForeverConnectionException);
                else
                    retryPolicy = policyBuilder.WaitAndRetry(_options.ConnectionRetryCount, r => TimeSpan.FromSeconds(3 * r), OnConnectionException);

                var policyResult = retryPolicy.ExecuteAndCapture(() => _connectionFactory.CreateConnection(_options.HostNames.ToList(), _options.ConnectionName));
                _connection = policyResult.Result;
                return _connection;

                void OnForeverConnectionException(Exception ex, int r, TimeSpan ts, Context ctx)
                {
                    logger.LogError(ex, "Retry [ {Retry} / {TotalRetry} ]. Error while connecting to RabbitMq with hostnames {@HostNames}. {ErrorMessage}", r, "∞", _options.HostNames, ex.Message);
                }

                void OnConnectionException(Exception ex, TimeSpan ts, int r, Context ctx)
                {
                    logger.LogError(ex, "Retry [ {Retry} / {TotalRetry} ]. Error while connecting to RabbitMq with hostnames {@HostNames} {ErrorMessage}", r, _options.ConnectionRetryCount, _options.HostNames, ex.Message);
                }
            }

        }
    }
}
