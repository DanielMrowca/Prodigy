using System.IO;
using System.IO.Compression;

namespace HoneyComb.RequestDecompression.Providers
{
    public class DeflateDecompressionProvider : IDecompressionProvider
    {
        public string EncodingName => "deflate";

        public Stream DecompressStream(Stream compressedStream)
            => new DeflateStream(compressedStream, CompressionMode.Decompress);
    }
}
