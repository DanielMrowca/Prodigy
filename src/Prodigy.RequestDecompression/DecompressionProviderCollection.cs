using System;
using System.Collections.ObjectModel;

namespace Prodigy.RequestDecompression
{
    public class DecompressionProviderCollection : Collection<IDecompressionProvider>
    {
        public void Add<TDecompressionProvider>() where TDecompressionProvider : IDecompressionProvider
        {
            Add(typeof(TDecompressionProvider));
        }

        public void Add(Type decompressionProviderType)
        {
            if (decompressionProviderType is null)
                throw new ArgumentNullException(nameof(decompressionProviderType));

            if (!typeof(IDecompressionProvider).IsAssignableFrom(decompressionProviderType))
                throw new ArgumentException($"The decompression provider must implement {nameof(IDecompressionProvider)}.", nameof(decompressionProviderType));

            var factory = new DecompressionProviderFactory(decompressionProviderType);
            Add(factory);
        }
    }
}
