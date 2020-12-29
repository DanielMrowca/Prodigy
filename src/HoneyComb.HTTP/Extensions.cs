using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.HTTP
{
    public static class Extensions
    {
        private const string SectionName = "httpClient";

        public static IHoneyCombBuilder AddHttpClient(this IHoneyCombBuilder builder, string clientName = "honeyComb", string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                sectionName = SectionName;

            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("HTTP client name cannot be empty", nameof(clientName));

            var options = builder.GetSettings<HttpClientOptions>(sectionName);
            builder.Services.AddSingleton(options);
            builder.Services.AddHttpClient<IHttpClient, HoneyCombHttpClient>(clientName);

            return builder;
        }

        public static string ToStringArray(this string[] array)
        {
            var idsAsStringArray = string.Join("','", array);
            return $"['{idsAsStringArray}']";
        }

        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }
    }
}
