using HoneyComb.Logging.Settings;
using HoneyComb.Models;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace HoneyComb.Logging
{
    public static class Extensions
    {
        private const string DefaultLoggerSectionName = "logger";
        private const string DefaultAppSectionName = "app";

        public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder,
            Action<LoggerConfiguration> configure = null, string loggerSectionName = DefaultLoggerSectionName,
            string appSectionName = DefaultAppSectionName)
        {
            return webHostBuilder.UseSerilog((context, loggerConfig) =>
            {
                if (string.IsNullOrWhiteSpace(loggerSectionName))
                    loggerSectionName = DefaultLoggerSectionName;
                if (string.IsNullOrWhiteSpace(appSectionName))
                    appSectionName = DefaultAppSectionName;


                var loggerSettings = context.Configuration.GetSettings<LoggerSettings>(loggerSectionName);
                var appSettings = context.Configuration.GetSettings<AppSettings>(appSectionName);

                BuildLoggerConfiguration(loggerConfig, loggerSettings, appSettings, context.HostingEnvironment.EnvironmentName);
                configure?.Invoke(loggerConfig);
            });
        }

        private static void BuildLoggerConfiguration(LoggerConfiguration loggerConfig,
            LoggerSettings loggerSettings, AppSettings appSettings, string environment)
        {
            if (!Enum.TryParse<LogEventLevel>(loggerSettings.Level, out var level))
                level = LogEventLevel.Information;

            loggerConfig.Enrich.FromLogContext()
                .MinimumLevel.Is(level)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithProperty("Application", appSettings.Name)
                .Enrich.WithProperty("Version", appSettings.Version);

            foreach (var prop in loggerSettings.Properties)
            {
                loggerConfig.Enrich.WithProperty(prop.Key, prop.Value);
            }

            Configure(loggerConfig, level, loggerSettings);
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
                loggerConfiguration.WriteTo.Seq(seqSettings.Url, apiKey: seqSettings.ApiKey);
            }
        }
    }
}
