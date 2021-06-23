using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.ContentResults
{
    public interface IFileResult
    {
        public byte[] FileContents { get; }
        public string ContentType { get; }
        public string FileDownloadName { get; set; }

        /// <summary>
        /// Gets or sets the last modified information associated with the <see cref="IFileResult"/>.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the etag associated with the <see cref="IFileResult"/>.
        /// </summary>
        public EntityTagHeaderValue? EntityTag { get; set; }
    }
}
