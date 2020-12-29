using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HoneyComb.HTTP
{
    public class CompressedHttpContent : HttpContent
    {
        private readonly HttpContent _originalContent;
        private readonly CompressionMethod _compressionMethod;

        public CompressedHttpContent(HttpContent content, CompressionMethod compressionMethod)
        {
            _originalContent = content ?? throw new ArgumentNullException(nameof(content));
            _compressionMethod = compressionMethod;

            foreach (KeyValuePair<string, IEnumerable<string>> header in _originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(_compressionMethod.ToString().ToLowerInvariant());
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (_compressionMethod == CompressionMethod.GZip)
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(gzipStream);
                }
            }
            else if (_compressionMethod == CompressionMethod.Deflate)
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(deflateStream);
                }
            }
            else if (_compressionMethod == CompressionMethod.Brotli)
            {
                using (var brotliStream = new BrotliStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(brotliStream);
                }
            }
        }
    }

    public enum CompressionMethod
    {
        GZip = 1,
        Deflate = 2,
        Brotli = 3
    }
}
