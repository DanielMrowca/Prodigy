using HoneyComb.Logging.Settings;
using HoneyComb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace HoneyComb.Logging
{
    public static class Extensions
    {
        private const string DefaultLoggerSectionName = "logger";
        private const string DefaultAppSectionName = "app";

        public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder,
            Action<WebHostBuilderContext, IServiceProvider, LoggerConfiguration> configure = null, string loggerSectionName = DefaultLoggerSectionName,
            string appSectionName = DefaultAppSectionName)
        {
            return webHostBuilder.UseSerilog((ctx, serviceProvider, loggerConfig) =>
            {
                if (string.IsNullOrWhiteSpace(loggerSectionName))
                    loggerSectionName = DefaultLoggerSectionName;
                if (string.IsNullOrWhiteSpace(appSectionName))
                    appSectionName = DefaultAppSectionName;


                var loggerSettings = ctx.Configuration.GetSettings<LoggerSettings>(loggerSectionName);
                var appSettings = ctx.Configuration.GetSettings<AppSettings>(appSectionName);

                BuildLoggerConfiguration(loggerConfig, loggerSettings, appSettings, ctx.HostingEnvironment.EnvironmentName);
                configure?.Invoke(ctx, serviceProvider, loggerConfig);
            });
        }

        public static IApplicationBuilder UseLogging(this IApplicationBuilder app,
            string loggerSectionName = DefaultLoggerSectionName)
        {
            var config = app.ApplicationServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var loggerSettings = config.GetSettings<LoggerSettings>(loggerSectionName);
            if (loggerSettings.UseCustomRequestLogging)
                app.UseSerilogRequestLogging(opt =>
                {
                    opt.GetLevel = (httpContext, elapsedMs, ex) =>
                    {
                        return LogEventLevel.Debug;
                    };
                });

            return app;
        }

        


        public static LoggerConfiguration BuildLoggerConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            return BuildLoggerConfiguration(new LoggerConfiguration(), config, environment);
        }

        public static LoggerConfiguration BuildLoggerConfiguration(LoggerConfiguration loggerConfig, IConfiguration configuration, string environmentName)
            => BuildLoggerConfiguration(loggerConfig,
                configuration.GetSettings<LoggerSettings>(DefaultLoggerSectionName),
                configuration.GetSettings<AppSettings>(DefaultAppSectionName),
                environmentName);

        public static LoggerConfiguration BuildLoggerConfiguration(LoggerConfiguration loggerConfig,
            LoggerSettings loggerSettings, AppSettings appSettings, string environment)
        {
            if (!Enum.TryParse<LogEventLevel>(loggerSettings.Level, out var level))
                level = LogEventLevel.Information;

            loggerConfig.Enrich.FromLogContext()
                .MinimumLevel.Is(level)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithProperty("Application", appSettings.Name)
                .Enrich.WithProperty("Version", appSettings.Version)
                .Enrich.WithProperty("VersionNumber", appSettings.VersionNumber);

            foreach (var prop in loggerSettings.Properties)
            {
                loggerConfig.Enrich.WithProperty(prop.Key, prop.Value);
            }

            ConfigureOverrides(loggerConfig, loggerSettings);
            Configure(loggerConfig, level, loggerSettings);
            return loggerConfig;
        }

        private static void Configure(LoggerConfiguration loggerConfiguration, LogEventLevel level,
            LoggerSettings settings)
        {
            var consoleSettings = settings.Console ?? new ConsoleSettings();
            var fileSettings = settings.File ?? new FileSettings();
            var seqSettings = settings.Seq ?? new SeqSettings();

            if (consoleSettings.IsEnabled)
                loggerConfiguration.WriteTo.ColoredConsole();

            if (fileSettings.IsEnabled)
            {
                var path = string.IsNullOrWhiteSpace(fileSettings.Path) ? "logs/log-{Date}.txt" : fileSettings.Path;
                loggerConfiguration.WriteTo.RollingFile(path, fileSizeLimitBytes: 10485760); //10485760 --> 10MB
            }

            if (seqSettings.IsEnabled)
            {
                loggerConfiguration.WriteTo.Seq(seqSettings.Url, restrictedToMinimumLevel: level, apiKey: seqSettings.ApiKey);
            }
        }

        private static void ConfigureOverrides(LoggerConfiguration loggerConfiguration, LoggerSettings settings)
        {
            if (settings.UseCustomRequestLogging)
                loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            foreach (var @override in settings.Override)
            {
                if (!Enum.TryParse<LogEventLevel>(@override.Value, out var level))
                    level = LogEventLevel.Information;

                loggerConfiguration.MinimumLevel.Override(@override.Key, level);
            }
        }
    }
}
