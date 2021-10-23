using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ModelBinding
{
    public interface IModelBinder
    {
        string ContentType { get; }
        Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class;
    }
}
