using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.Figlet;
using Prodigy.Models;
using Prodigy.Types;

namespace Prodigy
{
    public static class Extensions
    {
        private const string AppSectionName = "app";

        public static IProdigyBuilder AddProdigy(this IServiceCollection services, AppSettings appSettings)
            => BuildProdigy(services, appSettings);

        public static IProdigyBuilder AddProdigy(this IServiceCollection services, string sectionName = AppSectionName)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var appSettings = string.IsNullOrWhiteSpace(sectionName) ? null : config.GetSettings<AppSettings>(sectionName);
                return BuildProdigy(services, appSettings);
            }
        }

        public static IProdigyBuilder AddInitializer<T>(this IProdigyBuilder builder) where T : class, IInitializer
        {
            builder.Services.AddSingleton<IInitializer, T>();
            return builder;
        }

        private static IProdigyBuilder BuildProdigy(IServiceCollection services, AppSettings appSettings)
        {
            if (appSettings == null)
                appSettings = new AppSettings();

            var builder = ProdigyBuilder.Create(services);
            builder.Services.AddSingleton(appSettings);

            if (appSettings.DisplayBanner)
                DisplayBanner(appSettings);

            return builder;
        }

        public static IApplicationBuilder UseProdigy(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
                Task.Run(async () => await initializer.InitializeAsync()).GetAwaiter().GetResult();
            }

            return app;
        }

        public static T GetSettings<T>(this IProdigyBuilder builder, string sectionName)
            where T : new()
        {
            using (var serviceProvider = builder.Services.BuildServiceProvider())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                return config.GetSettings<T>(sectionName);
            }
        }

        public static T GetSettings<T>(this IServiceCollection services, string sectionName)
            where T : new()
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                return config.GetSettings<T>(sectionName);
            }
        }

        public static T GetSettings<T>(this IConfiguration config, string sectionName)
            where T : new()
        {
            var model = new T();
            config.GetSection(sectionName).Bind(model);
            return model;
        }

        public static IProdigyBuilder AddRange(this IProdigyBuilder builder, IServiceCollection services)
        {
            foreach (var service in services)
            {
                builder.Services.Add(service);
            }

            return builder;
        }

        private static void DisplayBanner(AppSettings appSettings)
        {
            var textToDisplay = string.IsNullOrWhiteSpace(appSettings.Title) ? appSettings.Name : appSettings.Title;
            Console.WriteLine(FiggleFonts.ANSI_Shadow.Render(textToDisplay));
            if (!string.IsNullOrWhiteSpace(appSettings.Subtitle))
                Console.WriteLine(FiggleFonts.ANSI_Shadow.Render(appSettings.Subtitle));
            Console.WriteLine(FiggleFonts.ANSI_Shadow.Render($"ver. {appSettings.Version}"));
            Console.WriteLine(FiggleFonts.ANSI_Shadow.Render($"[ {appSettings.VersionNumber} ]"));
        }
    }
}
