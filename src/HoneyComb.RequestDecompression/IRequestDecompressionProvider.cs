using Microsoft.AspNetCore.Http;

namespace HoneyComb.RequestDecompression
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
