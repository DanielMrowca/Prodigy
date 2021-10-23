using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ModelBinding.Binders
{
    internal class JsonModelBinder : ModelBinderBase
    {
        public override string ContentType => MediaTypeNames.Application.Json;

        public override async Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class
        {
            await base.BindModelAsync<T>(httpContext);
            return await httpContext.ReadJsonAsync<T>();
        }
    }
}
