using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Prodigy.RequestDecompression
{
    public class RequestDecompressionProvider : IRequestDecompressionProvider
    {
        private readonly IDecompressionProvider[] _providers;
        private readonly ILogger _logger;

        public string[] AcceptEncoding => _providers.Select(x => x.EncodingName).ToArray();

        /// <summary>
        ///     If no decompression providers are specified then all is used by default.
        /// </summary>
        /// <param name="services">Services to use when instantiating decompression providers.</param>
        /// <param name="options">The options for this instance.</param>
        public RequestDecompressionProvider(IServiceProvider services, RequestDecompressionOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _providers = options.Providers.ToArray();

            // If no decompression providers are specified then all is used by default
            if (_providers.Length == 0)
                _providers = GetAllAvailableDecompressionProviders();
            else
            {
                for (int i = 0; i < _providers.Length; i++)
                {
                    var factory = _providers[i] as DecompressionProviderFactory;
                    if (factory != null)
                    {
                        _providers[i] = factory.CreateInstance(services);
                    }
                }
            }

            _logger = services.GetRequiredService<ILogger<RequestDecompressionProvider>>();
        }

        public IDecompressionProvider GetDecompressionProvider(HttpContext context)
        {
            if (!ShouldDecompressRequest(context))
                return null;

            var contentEncoding = context.Request.Headers[HeaderNames.ContentEncoding];
            var provider = _providers.FirstOrDefault(x => x.EncodingName.Equals(contentEncoding, StringComparison.OrdinalIgnoreCase));
            _logger.LogDebug("The request will be decompressed with {DecompressionProvider}.", provider.EncodingName);
            return provider;
        }

        public bool ShouldDecompressRequest(HttpContext context)
        {
            var contentEncoding = context.Request.Headers[HeaderNames.ContentEncoding];

            if (string.IsNullOrWhiteSpace(contentEncoding))
            {
                _logger.LogDebug("No content encoding");
                return false;
            }

            var provider = _providers.FirstOrDefault(x => x.EncodingName.Equals(contentEncoding, StringComparison.OrdinalIgnoreCase));
            if (provider is null)
            {
                _logger.LogDebug("No matching request decompression provider found for content encoding {ContentEncoding}", contentEncoding);
                return false;
            }

            _logger.LogTrace("Should decompress request with provider {DecompressProvider}", provider.EncodingName);
            return true;
        }

        /// <summary>
        ///     Get all available decompression providers.
        /// </summary>
        /// <returns>Returning all available decompression providers.</returns>
        private static IDecompressionProvider[] GetAllAvailableDecompressionProviders()
            => Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(t => typeof(IDecompressionProvider).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && !t.IsAssignableFrom(typeof(DecompressionProviderFactory)))
               .Select(t => (IDecompressionProvider)Activator.CreateInstance(t))
               .ToArray();
    }
}
