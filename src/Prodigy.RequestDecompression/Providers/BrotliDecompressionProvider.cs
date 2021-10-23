using System.IO;
using System.IO.Compression;

namespace Prodigy.RequestDecompression.Providers
{
    public class BrotliDecompressionProvider : IDecompressionProvider
    {
        public string EncodingName => "br";

        public Stream DecompressStream(Stream compressedStream)
            => new BrotliStream(compressedStream, CompressionMode.Decompress);
    }
}
