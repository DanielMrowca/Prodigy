using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.ContentResults
{
    public interface IHttpResponseContentResult
    {
        Task WriteContentAsync(HttpContext httpContext);
    }
}
