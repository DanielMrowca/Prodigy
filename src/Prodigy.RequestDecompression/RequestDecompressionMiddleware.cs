using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Prodigy.RequestDecompression
{
    public sealed class RequestDecompressionMiddleware
    {
        private readonly IRequestDecompressionProvider _provider;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestDecompressionMiddleware(IRequestDecompressionProvider provider, RequestDelegate next, IServiceProvider services)
        {
            _provider = provider;
            _next = next;
            _logger = services.GetRequiredService<ILogger<RequestDecompressionMiddleware>>();
        }

        public async Task Invoke(HttpContext context)
        {
            var decompressionProvider = _provider.GetDecompressionProvider(context);
            if (decompressionProvider is null)
            {
                await _next(context);
                return;
            }

            context.Request.Body = decompressionProvider.DecompressStream(context.Request.Body);
            await _next(context);
        }
    }
}
