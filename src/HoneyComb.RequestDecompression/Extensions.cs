using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HoneyComb.RequestDecompression
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddRequestDecompression(this IHoneyCombBuilder builder, Action<RequestDecompressionOptions> configureOptions = null)
        {
            var options = new RequestDecompressionOptions();
            configureOptions?.Invoke(options);

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<IRequestDecompressionProvider, RequestDecompressionProvider>();

            return builder;
        }

        public static IApplicationBuilder UseRequestDecompression(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<RequestDecompressionMiddleware>();
        }
    }
}
