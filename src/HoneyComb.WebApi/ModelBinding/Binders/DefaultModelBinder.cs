using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HoneyComb.WebApi.ModelBinding.Binders
{
    /// <summary>
    ///     Default model binder for unknown or empty Content-Type
    /// </summary>
    internal class DefaultModelBinder : IModelBinder
    {
        public string ContentType => string.Empty;

        public Task<T> BindModelAsync<T>(HttpContext httpContext) where T : class
            => httpContext.ReadJsonAsync<T>();
    }
}
