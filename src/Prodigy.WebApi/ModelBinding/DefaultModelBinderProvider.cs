using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Prodigy.WebApi.ModelBinding.Binders;

namespace Prodigy.WebApi.ModelBinding
{
    public class DefaultModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetModelBinder(HttpContext httpContext)
        {
            var contentType = httpContext.Request.ContentType;
            var contentLength = httpContext.Request.ContentLength;
            var isEmptyContent = contentLength is null || contentLength is 0;

            if (contentType is null || isEmptyContent)
                return new DefaultModelBinder();
            if ( contentType.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase))
                return new JsonModelBinder();
            if(contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                return new FormFileModelBinder();

            throw new NotSupportedException($"Content type: {contentType} is not supported.");

        }
    }
}
