using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prodigy.HTTP
{
    public static class Extensions
    {
        private const string SectionName = "httpClient";

        public static IProdigyBuilder AddHttpClient(this IProdigyBuilder builder, string clientName = "prodigy", string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                sectionName = SectionName;

            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("HTTP client name cannot be empty", nameof(clientName));

            var options = builder.GetSettings<HttpClientOptions>(sectionName);
            builder.Services.AddSingleton(options);
            builder.Services.AddHttpClient<IHttpClient, ProdigyHttpClient>(clientName);

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
