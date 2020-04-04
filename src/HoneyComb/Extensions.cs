using HoneyComb.Models;
using HoneyComb.Figlet;
using HoneyComb.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HoneyComb
{
    public static class Extensions
    {
        private const string AppSectionName = "app";

        public static IHoneyCombBuilder AddHoneyComb(this IServiceCollection services, AppSettings appSettings)
            => BuildHoneyComb(services, appSettings);

        public static IHoneyCombBuilder AddHoneyComb(this IServiceCollection services, string sectionName = AppSectionName)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var appSettings = string.IsNullOrWhiteSpace(sectionName) ? null : config.GetSettings<AppSettings>(sectionName);
                return BuildHoneyComb(services, appSettings);
            }
        }

        private static IHoneyCombBuilder BuildHoneyComb(IServiceCollection services, AppSettings appSettings)
        {
            if (appSettings == null)
                appSettings = new AppSettings();

            var builder = HoneyCombBuilder.Create(services);
            builder.Services.AddSingleton(appSettings);

            if (appSettings.DisplayBanner)
                Console.WriteLine(HoneyComb.Figlet.FiggleFonts.ANSI_Shadow.Render($"{appSettings.Name} v{appSettings.Version}"));

            return builder;
        }

        public static IApplicationBuilder UseHoneyComb(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
                Task.Run(async () => await initializer.InitializeAsync()).GetAwaiter().GetResult();
            }

            return app;
        }

        public static T GetSettings<T>(this IHoneyCombBuilder builder, string sectionName)
            where T : new()
        {
            using (var serviceProvicer = builder.Services.BuildServiceProvider())
            {
                var config = serviceProvicer.GetRequiredService<IConfiguration>();
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
    }
}
