namespace Prodigy.RequestDecompression
{
    /// <summary>
    ///     Options for HTTP decompression middleware
    /// </summary>
    public class RequestDecompressionOptions
    {
        /// <summary>
        ///     The <see cref="IDecompressionProvider"/> types to use for requests.
        /// </summary>
        public DecompressionProviderCollection Providers { get; } = new DecompressionProviderCollection();
    }
}
