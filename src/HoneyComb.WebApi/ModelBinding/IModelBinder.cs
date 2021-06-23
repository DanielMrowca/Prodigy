using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.ModelBinding
{
    public interface IModelBinder
    {
        string ContentType { get; }
        Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class;
    }
}
