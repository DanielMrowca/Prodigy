using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ContentResults
{
    public interface IHttpResponseContentResult
    {
        Task WriteContentAsync(HttpContext httpContext);
    }
}
