using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ModelBinding.Binders
{
    public abstract class ModelBinderBase : IModelBinder
    {
        public abstract string ContentType { get; }

        public virtual Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class
        {
            var contentType = httpContext.Request.ContentType;
            if (!contentType.Contains(ContentType, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Http content type: {contentType} is not valid for {nameof(JsonModelBinder)}");

            return Task.FromResult((T)default);
        }
    }
}
