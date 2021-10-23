using Microsoft.AspNetCore.Http;

namespace Prodigy.RequestDecompression
{
    public interface IRequestDecompressionProvider
    {
        /// <summary>
        ///     Return list of accepted encoding depend on defined decompression providers.
        /// </summary>
        string[] AcceptEncoding { get; }
        IDecompressionProvider? GetDecompressionProvider(HttpContext context);
        bool ShouldDecompressRequest(HttpContext context);
    }
}
