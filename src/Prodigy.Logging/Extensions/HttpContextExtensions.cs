using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Prodigy.Logging
{
    public static class HttpContextExtensions
    {
        public static string GetHeaderValue(this HttpContext httpContext, string headerName)
        {
            if (httpContext is null)
                throw new ArgumentNullException(nameof(httpContext));
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentNullException(nameof(headerName));

            if (!httpContext.Request.Headers.TryGetValue(headerName, out StringValues values))
                return null;

            return values.FirstOrDefault();
        }
    }
}
