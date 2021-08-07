﻿using HoneyComb.Models;
using HoneyComb.Figlet;
using HoneyComb.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

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

        public static IHoneyCombBuilder AddInitializer<T>(this IHoneyCombBuilder builder) where T : class, IInitializer
        {
            builder.Services.AddSingleton<IInitializer, T>();
            return builder;
        }

        private static IHoneyCombBuilder BuildHoneyComb(IServiceCollection services, AppSettings appSettings)
        {
            if (appSettings == null)
                appSettings = new AppSettings();

            var builder = HoneyCombBuilder.Create(services);
            builder.Services.AddSingleton(appSettings);

            if (appSettings.DisplayBanner)
                DisplayBanner(appSettings);

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

        public static T GetSettings<T>(this IServiceCollection services, string sectionName)
            where T : new()
        {
            using (var serviceProvicer = services.BuildServiceProvider())
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

        public static IHoneyCombBuilder AddRange(this IHoneyCombBuilder builder, IServiceCollection services)
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
