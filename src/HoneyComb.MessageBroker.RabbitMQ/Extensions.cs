using HoneyComb.MessageBroker.RabbitMQ.Clients;
using HoneyComb.MessageBroker.RabbitMQ.Conventions;
using HoneyComb.MessageBroker.RabbitMQ.Factories;
using HoneyComb.MessageBroker.RabbitMQ.Initializers;
using HoneyComb.MessageBroker.RabbitMQ.Publishers;
using HoneyComb.MessageBroker.RabbitMQ.Subscribers;
using HoneyComb.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Open.Serialization.Json;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddRabbitMQ(this IHoneyCombBuilder builder, RabbitMqOptions options, IJsonSerializer jsonSerializer = null,
            IConventionBuilder conventionBuilder = null, IRabbitQueuePrefixProvider rabbitQueueIdentifierProvider = null, int retriesCountOnConnectingFailed = -1)
        {
            var jsonSerializerIsRegistered = builder.Services.Any(x => x.ServiceType == typeof(IJsonSerializer));
            if (!jsonSerializerIsRegistered)
            {
                if(jsonSerializer is null)
                {
                    var factory = new Open.Serialization.Json.Newtonsoft.JsonSerializerFactory(new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    jsonSerializer = factory.GetSerializer();
                }
                builder.Services.AddSingleton(jsonSerializer);
            }

            var queueIdentIsRegistered = builder.Services.Any(x => x.ServiceType == typeof(IRabbitQueuePrefixProvider));
            if (!queueIdentIsRegistered)
            {
                if (rabbitQueueIdentifierProvider is null)
                    builder.Services.AddSingleton<IRabbitQueuePrefixProvider, RabbitQueueIdentifierProvider>();
                else
                    builder.Services.AddSingleton(rabbitQueueIdentifierProvider);
            }

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<IConventionBuilder, UnderscoreCaseConventionBuilder>();

            if (conventionBuilder is null)
                builder.Services.AddSingleton<IConventionProvider, ConventionProvider>();
            else
                builder.Services.AddSingleton(conventionBuilder);

            builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            builder.Services.AddSingleton<IBusPublisher, RabbitMqPublisher>();
            builder.Services.AddSingleton<IBusSubscriber, RabbitMqSubscriber>();

            builder.Services.AddTransient<IInitializer, RabbitMqExchangeInitializer>();

            var connectionFactory = new ConnectionFactory
            {
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.Username,
                Password = options.Password,
                RequestedConnectionTimeout = TimeSpan.FromMilliseconds(options.RequestedConnectionTimeout), //ms
                SocketReadTimeout = TimeSpan.FromMilliseconds(options.SocketReadTimeout), //ms
                SocketWriteTimeout = TimeSpan.FromMilliseconds(options.SocketWriteTimeout), //ms
                RequestedChannelMax = options.RequestedChannelMax,
                RequestedFrameMax = options.RequestedFrameMax,
                RequestedHeartbeat = TimeSpan.FromSeconds(options.RequestedHeartbeat), //sec
                UseBackgroundThreadsForIO = options.UseBackgroundThreadsForIO,
                DispatchConsumersAsync = true,
                Ssl = options.Ssl is null
                    ? new SslOption()
                    : new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath, options.Ssl.Enabled)

            };


            builder.Services.AddSingleton(connectionFactory);
            builder.Services.AddSingleton<IConnectionFactory>(new ConnectionWithRetryFactory(connectionFactory, options, builder));


            //var policyBuilder = Policy.Handle<Exception>();
            //RetryPolicy retryPolicy = null;

            //if (retriesCountOnConnectingFailed < 0)
            //    retryPolicy = policyBuilder.WaitAndRetryForever(r => TimeSpan.FromSeconds(3 * r), OnConnectionException);
            //else
            //    retryPolicy = policyBuilder.WaitAndRetry(retriesCountOnConnectingFailed, r => TimeSpan.FromSeconds(3 * r), OnConnectionException);

            //var connection = retryPolicy.ExecuteAndCapture(() => connectionFactory.CreateConnection(options.HostNames.ToList(), options.ConnectionName));

            //void OnConnectionException(Exception ex, TimeSpan ts)
            //{
            //    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<IConnection>>();
            //    logger.LogError(ex, $"Error while connecting to RabbitMq. {ex.Message}");
            //}

            //builder.Services.AddSingleton(connection);


            return builder;
        }

        public static IHoneyCombBuilder AddRabbitMQ(this IHoneyCombBuilder builder, string sectionName, string connectionName = null, string prefixQueueName = null, int retriesCountOnConnectingFailed = -1)
        {
            var options = builder.GetSettings<RabbitMqOptions>(sectionName);
            options.ConnectionName = string.IsNullOrWhiteSpace(connectionName) ? options.ConnectionName : connectionName;
            var queueIdentProvider = string.IsNullOrWhiteSpace(prefixQueueName) ? null : new RabbitQueueIdentifierProvider(prefixQueueName);

            return AddRabbitMQ(builder, options, null, null, queueIdentProvider, retriesCountOnConnectingFailed);
        }


        public static IBusSubscriber UseRabbitMQ(this IApplicationBuilder app)
            => new RabbitMqSubscriber(app.ApplicationServices);
    }
}
