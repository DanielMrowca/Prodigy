using System.IO;

namespace Prodigy.RequestDecompression
{
    public interface IDecompressionProvider
    {
        /// <summary>
        ///     The encoding name used in the 'Accept-Encoding' response header and 'Content-Encoding' request header.
        /// </summary>
        string EncodingName { get; }

        /// <summary>
        ///     Create new decompressed stream
        /// </summary>
        /// <param name="compressedStream">The stream where the decompressed data have to be written.</param>
        /// <returns>The decompressed stream.</returns>
        Stream DecompressStream(Stream compressedStream);
    }
}
