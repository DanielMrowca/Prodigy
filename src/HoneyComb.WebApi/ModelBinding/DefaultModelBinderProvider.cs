using HoneyComb.WebApi.ModelBinding.Binders;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;

namespace HoneyComb.WebApi.ModelBinding
{
    public class DefaultModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetModelBinder(HttpContext httpContext)
        {
            var contentType = httpContext.Request.ContentType;

            if (contentType is null || contentType.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase))
                return new JsonModelBinder();
            if(contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                return new FormFileModelBinder();

            throw new NotSupportedException($"Content type: {contentType} is not supported.");

        }
    }
}
