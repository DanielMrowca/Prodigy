using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Prodigy.RequestDecompression
{
    public static class Extensions
    {
        public static IProdigyBuilder AddRequestDecompression(this IProdigyBuilder builder, Action<RequestDecompressionOptions> configureOptions = null)
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
