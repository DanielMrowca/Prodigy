using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.ModelBinding.Binders
{
    internal class JsonModelBinder : ModelBinderBase
    {
        public override string ContentType => MediaTypeNames.Application.Json;

        public override async Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class
        {
            var contentType = httpContext.Request.ContentType;

            // Validate content type only when is specified in the request
            if (!string.IsNullOrWhiteSpace(contentType))
                await base.BindModelAsync<T>(httpContext);

            return await httpContext.ReadJsonAsync<T>();
        }
    }
}
