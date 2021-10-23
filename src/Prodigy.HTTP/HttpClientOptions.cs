using System.Collections.Generic;

namespace Prodigy.HTTP
{
    public class HttpClientOptions
    {
        public string Type { get; set; }
        public int Retries { get; set; }
        public IDictionary<string, string> Services { get; set; }
        public RequestMaskingOptions RequestMasking { get; set; }
        public CompressionOptions Compression { get; set; } = new CompressionOptions();

        public class RequestMaskingOptions
        {
            public bool Enabled { get; set; }
            public IEnumerable<string> UrlParts { get; set; }
            public string MaskTemplate { get; set; }
        }

        public class CompressionOptions
        {
            public bool IsEnabled { get; set; } = false;
            public string Method { get; set; } = CompressionMethod.GZip.ToString();
        }
    }
}
