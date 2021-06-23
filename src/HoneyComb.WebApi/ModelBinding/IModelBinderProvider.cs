using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.ModelBinding
{
    public interface IModelBinderProvider
    {
        IModelBinder GetModelBinder(HttpContext httpContext);
    }
}
