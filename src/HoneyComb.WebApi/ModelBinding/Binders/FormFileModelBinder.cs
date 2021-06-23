using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.ModelBinding.Binders
{
    internal class FormFileModelBinder : ModelBinderBase
    {
        public override string ContentType => "multipart/form-data";

        public override async Task<T> BindModelAsync<T>(HttpContext httpContext)
        {
            await base.BindModelAsync<T>(httpContext);

            if (!(typeof(IFormFile).IsAssignableFrom(typeof(T))))
                return null;

            var request = httpContext.Request;
            if (!request.HasFormContentType)
                return null;

            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            var model = (T)Activator.CreateInstance(typeof(T), new object[] { file.OpenReadStream(), 0, file.Length, file.Name, file.FileName });
            return model;

        }
    }
}
