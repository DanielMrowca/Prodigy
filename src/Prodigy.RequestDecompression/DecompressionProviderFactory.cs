using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Prodigy.RequestDecompression
{
    /// <summary>
    ///     This is a placeholder for the DecompressionProviderCollection that allows creating the given type vie
    ///     an <see cref="IServiceProvider"/>
    /// </summary>
    public class DecompressionProviderFactory : IDecompressionProvider
    {
        private readonly Type _decompressionProviderType;

        public DecompressionProviderFactory(Type decompressionProviderType)
        {
            _decompressionProviderType = decompressionProviderType;
        }

        public IDecompressionProvider CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var t = (IDecompressionProvider)ActivatorUtilities.CreateInstance(serviceProvider, _decompressionProviderType, Type.EmptyTypes);

            return t;
        }

        string IDecompressionProvider.EncodingName => throw new NotSupportedException();
        Stream IDecompressionProvider.DecompressStream(Stream compressedStream)
        {
            throw new NotSupportedException();
        }
    }
}
