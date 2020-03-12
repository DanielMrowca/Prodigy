using HoneyComb.WebApi.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HoneyComb.WebApi
{
    public static class Extensions
    {
        private const string EmptyJsonObject = "{}";
        private const string JsonContentType = "application/json";
        private static readonly byte[] InvalidJsonRequestBytes = Encoding.UTF8.GetBytes("An invalid JSON was sent.");
        private static bool _bindRequestFromRoute;

        public static IHoneyCombBuilder AddWebApi(this IHoneyCombBuilder builder, Action<IMvcCoreBuilder> configureMvc = null)
        {
            //TODO ADD required services for WebApi ;)

            var mvcCoreBuilder = builder.Services
               .AddLogging()
               .AddMvcCore();

            configureMvc?.Invoke(mvcCoreBuilder);

            return builder;
        }

        public static IHoneyCombBuilder AddErrorHandler<T>(this IHoneyCombBuilder builder) where T : class, IExceptionToResponseMapper
        {
            builder.Services.AddTransient<IExceptionToResponseMapper, T>();
            return builder;
        }

        public static IHoneyCombBuilder AddErrorHandler(this IHoneyCombBuilder builder)
        {
            builder.Services.AddTransient<ErrorHandlerMiddleware>();
            return builder;
        }

        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder appBuilder, Action<IEndpointBuilder> builder)
        {
            appBuilder.UseRouting();
            appBuilder.UseEndpoints(router => builder?.Invoke(new EndpointBuilder(router)));
            return appBuilder;
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();

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

        public static async Task<T> ReadJsonAsync<T>(this HttpContext httpContext)
        {
            if (httpContext.Request.Body is null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.Body.WriteAsync(InvalidJsonRequestBytes, 0, InvalidJsonRequestBytes.Length);

                return default;
            }

            try
            {
                var request = httpContext.Request;
                var payload = JsonConvert.DeserializeObject<T>(await request.Body.GetAsStringAsync());
                //var payload = await httpContext.RequestServices.GetRequiredService<IJsonSerializer>().DeserializeAsync<T>(request.Body);
                if (_bindRequestFromRoute && HasRouteData(request))
                {
                    var values = request.HttpContext.GetRouteData().Values;
                    foreach (var (key, value) in values)
                    {
                        var field = payload.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                            .SingleOrDefault(f => f.Name.ToLowerInvariant().StartsWith($"<{key}>",
                                StringComparison.InvariantCultureIgnoreCase));

                        if (field is null)
                        {
                            continue;
                        }

                        var fieldValue = TypeDescriptor.GetConverter(field.FieldType)
                            .ConvertFromInvariantString(value.ToString());
                        field.SetValue(payload, fieldValue);
                    }
                }

                var results = new List<ValidationResult>();
                if (Validator.TryValidateObject(payload, new ValidationContext(payload), results))
                {
                    return payload;
                }

                httpContext.Response.StatusCode = 400;
                httpContext.Response.WriteJsonAsync(results);

                return default;
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.Body.WriteAsync(InvalidJsonRequestBytes, 0, InvalidJsonRequestBytes.Length);

                return default;
            }
        }

        public static async Task<string> GetAsStringAsync(this Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return await sr.ReadToEndAsync();
            }
        }

        public static Stream GetAsStream(this string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return new MemoryStream(bytes);
        }

        public static void WriteJsonAsync<T>(this HttpResponse response, T value)
        {
            response.ContentType = JsonContentType;
            var data = JsonConvert.SerializeObject(value);
            response.Body = data.GetAsStream();

            //var serializer = response.HttpContext.RequestServices.GetRequiredService<IJsonSerializer>();
            //await serializer.SerializeAsync(response.Body, value);
        }

        private static bool HasQueryString(this HttpRequest request)
           => request.Query.Any();

        private static bool HasRouteData(this HttpRequest request)
           => request.HttpContext.GetRouteData().Values.Any();

    }
}
