using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HoneyComb.WebApi
{
    public static class Extensions
    {
        private const string EmptyJsonObject = "{}";

        public static IHoneyCombBuilder AddWebApi(this IHoneyCombBuilder builder, Action<IMvcCoreBuilder> configureMvc = null)
        {
            //TODO ADD required services for WebApi ;)

            var mvcCoreBuilder = builder.Services
               .AddLogging()
               .AddMvcCore();

            configureMvc?.Invoke(mvcCoreBuilder);

            return builder;
        }

        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder appBuilder, Action<IEndpointBuilder> builder)
        {

            appBuilder.UseRouting();
            appBuilder.UseEndpoints(router => builder?.Invoke(new EndpointBuilder(router)));
            return appBuilder;
        }

        public static T ReadQuery<T>(this HttpContext context) where T : class
        {
            var request = context.Request;
            RouteValueDictionary values = null;
            if (HasRouteData(request))
            {
                values = request.HttpContext.GetRouteData().Values;
            }

            if (HasQueryString(request))
            {
                var queryString = HttpUtility.ParseQueryString(request.HttpContext.Request.QueryString.Value);
                values ??= new RouteValueDictionary();
                foreach (var key in queryString.AllKeys)
                {
                    values.TryAdd(key, queryString[key]);
                }
            }

            if (values is null)
            {
                return JsonConvert.DeserializeObject<T>(EmptyJsonObject);
            }

            var serialized = JsonConvert.SerializeObject(values.ToDictionary(k => k.Key, k => k.Value))
                .Replace("\\\"", "\"")
                .Replace("\"{", "{")
                .Replace("}\"", "}")
                .Replace("\"[", "[")
                .Replace("]\"", "]");

            return JsonConvert.DeserializeObject<T>(serialized);
        }

        private static bool HasQueryString(this HttpRequest request)
           => request.Query.Any();

        private static bool HasRouteData(this HttpRequest request)
           => request.HttpContext.GetRouteData().Values.Any();

    }
}
