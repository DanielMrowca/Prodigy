using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ModelBinding
{
    public interface IModelBinderProvider
    {
        IModelBinder GetModelBinder(HttpContext httpContext);
    }
}
