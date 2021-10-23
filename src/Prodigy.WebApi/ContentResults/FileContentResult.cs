using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Prodigy.WebApi.ContentResults
{
    /// <summary>
    ///     Represent <see cref="IHttpResponseContentResult"/> that when executed will
    ///     write a binary file to the response
    ///     <para>Inspired by <see href="https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Core/src/FileContentResult.cs"/> </para>
    /// </summary>
    public class FileContentResult : IHttpResponseContentResult, IFileResult
    {
        private string? _fileDownloadName;

        public byte[] FileContents { get; }

        /// <summary>
        /// Gets the Content-Type header for the response.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the file name that will be used in the Content-Disposition header of the response.
        /// </summary>
        [AllowNull]
        public string FileDownloadName
        {
            get { return _fileDownloadName ?? string.Empty; }
            set { _fileDownloadName = value; }
        }

        /// <summary>
        /// Gets or sets the last modified information associated with the <see cref="IFileResult"/>.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the etag associated with the <see cref="FileResult"/>.
        /// </summary>
        public EntityTagHeaderValue? EntityTag { get; set; }

        public FileContentResult(byte[] fileContents, string contentType)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException(nameof(fileContents));
            }

            FileContents = fileContents;
            ContentType = MediaTypeHeaderValue.Parse(contentType).ToString();
        }

        public async Task WriteContentAsync(HttpContext httpContext)
        {
            if (httpContext is null)
                throw new ArgumentNullException(nameof(httpContext));

            SetHeaders(httpContext, this, FileContents.Length, LastModified, EntityTag);

            var fileContentStream = new MemoryStream(FileContents);
            await httpContext.WriteFileAsync(fileContentStream);
        }

        protected virtual void SetHeaders(
            HttpContext httpContext,
            IFileResult result,
            long? fileLength,
            DateTimeOffset? lastModified,
            EntityTagHeaderValue? etag)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            //var request = httpContext.Request;
            //var httpRequestHeaders = request.GetTypedHeaders();

            // Since the 'Last-Modified' and other similar http date headers are rounded down to whole seconds,
            // round down current file's last modified to whole seconds for correct comparison.
            if (lastModified.HasValue)
                lastModified = RoundDownToWholeSeconds(lastModified.Value);

            //var preconditionState = GetPreconditionState(httpRequestHeaders, lastModified, etag, logger);

            var response = httpContext.Response;
            SetLastModifiedAndEtagHeaders(response, lastModified, etag);

            // Short circuit if the preconditional headers process to 304 (NotModified) or 412 (PreconditionFailed)
            //if (preconditionState == PreconditionState.NotModified)
            //{
            //    response.StatusCode = StatusCodes.Status304NotModified;
            //    return (range: null, rangeLength: 0, serveBody: false);
            //}
            //else if (preconditionState == PreconditionState.PreconditionFailed)
            //{
            //    response.StatusCode = StatusCodes.Status412PreconditionFailed;
            //    return (range: null, rangeLength: 0, serveBody: false);
            //}

            SetContentType(httpContext, result);
            SetContentDispositionHeader(httpContext, result);

            if (fileLength.HasValue)
                response.ContentLength = fileLength.Value;

        }

        private static DateTimeOffset RoundDownToWholeSeconds(DateTimeOffset dateTimeOffset)
        {
            var ticksToRemove = dateTimeOffset.Ticks % TimeSpan.TicksPerSecond;
            return dateTimeOffset.Subtract(TimeSpan.FromTicks(ticksToRemove));
        }

        private static void SetLastModifiedAndEtagHeaders(HttpResponse response, DateTimeOffset? lastModified, EntityTagHeaderValue? etag)
        {
            var httpResponseHeaders = response.GetTypedHeaders();
            if (lastModified.HasValue)
            {
                httpResponseHeaders.LastModified = lastModified;
            }
            if (etag != null)
            {
                httpResponseHeaders.ETag = etag;
            }
        }

        private static void SetContentType(HttpContext httpContext, IFileResult result)
        {
            var response = httpContext.Response;
            response.ContentType = result.ContentType;
        }

        private static void SetContentDispositionHeader(HttpContext httpContext, IFileResult result)
        {
            if (!string.IsNullOrEmpty(result.FileDownloadName))
            {
                // From RFC 2183, Sec. 2.3:
                // The sender may want to suggest a filename to be used if the entity is
                // detached and stored in a separate file. If the receiving MUA writes
                // the entity to a file, the suggested filename should be used as a
                // basis for the actual filename, where possible.
                var contentDisposition = new ContentDispositionHeaderValue("attachment");
                contentDisposition.SetHttpFileName(result.FileDownloadName);
                httpContext.Response.Headers.Add(HeaderNames.ContentDisposition, contentDisposition.ToString());
            }
        }
    }
}
