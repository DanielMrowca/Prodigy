using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Hosting;

namespace HoneyComb.Logging
{
    public static class SerilogBuilderExtensions
    {
        public static IWebHostBuilder UseSerilog(
             this IWebHostBuilder builder,
             Action<WebHostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger,
             bool preserveStaticLogger = false,
             bool writeToProviders = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureLogger == null) throw new ArgumentNullException(nameof(configureLogger));

            builder.ConfigureServices((context, collection) =>
            {
                var loggerConfiguration = new LoggerConfiguration();

                LoggerProviderCollection loggerProviders = null;
                if (writeToProviders)
                {
                    loggerProviders = new LoggerProviderCollection();
                    loggerConfiguration.WriteTo.Providers(loggerProviders);
                }

                configureLogger(context, collection.BuildServiceProvider(), loggerConfiguration);
                var logger = loggerConfiguration.CreateLogger();

                ILogger registeredLogger = null;
                if (preserveStaticLogger)
                {
                    registeredLogger = logger;
                }
                else
                {
                    // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
                    // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
                    Log.Logger = logger;
                }

                collection.AddSingleton<ILoggerFactory>(services =>
                {
                    var factory = new SerilogLoggerFactory(registeredLogger, true, loggerProviders);

                    if (writeToProviders)
                    {
                        foreach (var provider in services.GetServices<ILoggerProvider>())
                            factory.AddProvider(provider);
                    }

                    return factory;
                });

                ConfigureServices(collection, logger);
            });
            return builder;
        }

        static void ConfigureServices(IServiceCollection collection, ILogger logger)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            if (logger != null)
            {
                // This won't (and shouldn't) take ownership of the logger. 
                collection.AddSingleton(logger);
            }

            // Registered to provide two services...
            var diagnosticContext = new DiagnosticContext(logger);

            // Consumed by e.g. middleware
            collection.AddSingleton(diagnosticContext);

            // Consumed by user code
            collection.AddSingleton<IDiagnosticContext>(diagnosticContext);
        }
    }
}

