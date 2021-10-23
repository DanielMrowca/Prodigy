using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Prodigy.WebApi.ContentResults
{
    public class JsonContentResult : IHttpResponseContentResult
    {
        public object Value { get; }

        public JsonContentResult(object value)
        {
            Value = value;
        }

        public async Task WriteContentAsync(HttpContext httpContext)
        {
            if (httpContext is null)
                throw new ArgumentNullException(nameof(httpContext));

            await httpContext.Response.WriteJsonAsync(Value);
        }
    }
}
