using HoneyComb.MessageBroker.RabbitMQ.Clients;
using HoneyComb.MessageBroker.RabbitMQ.Conventions;
using HoneyComb.MessageBroker.RabbitMQ.Initializers;
using HoneyComb.MessageBroker.RabbitMQ.Publishers;
using HoneyComb.MessageBroker.RabbitMQ.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Open.Serialization.Json;
using RabbitMQ.Client;
using System.Linq;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddRabbitMQ(this IHoneyCombBuilder builder, RabbitMqOptions options, IJsonSerializer jsonSerializer = null,
            IConventionBuilder conventionBuilder = null, IRabbitQueueIdentifierProvider rabbitQueueIdentifierProvider = null)
        {
            if (jsonSerializer is null)
            {
                var factory = new Open.Serialization.Json.Newtonsoft.JsonSerializerFactory(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                jsonSerializer = factory.GetSerializer();
            }

            if (rabbitQueueIdentifierProvider is null)
                builder.Services.AddSingleton<IRabbitQueueIdentifierProvider, RabbitQueueIdentifierProvider>();
            else
                builder.Services.AddSingleton(rabbitQueueIdentifierProvider);


            builder.Services.AddSingleton(jsonSerializer);
            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<IConventionBuilder, UnderscoreCaseConventionBuilder>();

            if (conventionBuilder is null)
                builder.Services.AddSingleton<IConventionProvider, ConventionProvider>();
            else
                builder.Services.AddSingleton(conventionBuilder);

            builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            builder.Services.AddSingleton<IBusPublisher, RabbitMqPublisher>();
            builder.Services.AddSingleton<IBusSubscriber, RabbitMqSubscriber>();

            builder.Services.AddTransient<RabbitMqExchangeInitializer>();
            builder.AddInitializer<RabbitMqExchangeInitializer>();

            var connection = new ConnectionFactory
            {
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.Username,
                Password = options.Password,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                SocketReadTimeout = options.SocketReadTimeout,
                SocketWriteTimeout = options.SocketWriteTimeout,
                RequestedChannelMax = options.RequestedChannelMax,
                RequestedFrameMax = options.RequestedFrameMax,
                RequestedHeartbeat = options.RequestedHeartbeat,
                UseBackgroundThreadsForIO = options.UseBackgroundThreadsForIO,
                DispatchConsumersAsync = true,
                Ssl = options.Ssl is null
                    ? new SslOption()
                    : new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath, options.Ssl.Enabled)

            }.CreateConnection(options.HostNames.ToList(), options.ConnectionName);
            builder.Services.AddSingleton(connection);

            return builder;
        }

        public static IHoneyCombBuilder AddRabbitMQ(this IHoneyCombBuilder builder, string sectionName, string connectionName = null, IJsonSerializer jsonSerializer = null,
            IConventionBuilder conventionBuilder = null, IRabbitQueueIdentifierProvider rabbitQueueIdentifierProvider = null)
        {
            var options = builder.GetSettings<RabbitMqOptions>(sectionName);
            options.ConnectionName = string.IsNullOrWhiteSpace(connectionName) ? options.ConnectionName : connectionName;
            return AddRabbitMQ(builder, options, jsonSerializer, conventionBuilder, rabbitQueueIdentifierProvider);
        }

        public static IBusSubscriber UseRabbitMQ(this IApplicationBuilder app)
            => new RabbitMqSubscriber(app.ApplicationServices);
    }
}
