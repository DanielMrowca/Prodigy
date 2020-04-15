using HoneyComb.WebApi.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Open.Serialization.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
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
        private const string LocationHeader = "Location";
        private static bool _bindRequestFromRoute = true;

        public static IHoneyCombBuilder AddWebApi(this IHoneyCombBuilder builder, Action<IMvcCoreBuilder> configureMvc = null,
            IJsonSerializer jsonSerializer = null)
        {
            if (jsonSerializer is null)
            {
                var factory = new Open.Serialization.Json.Newtonsoft.JsonSerializerFactory(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter(true) }
                });
                jsonSerializer = factory.GetSerializer();
            }

            builder.Services.AddSingleton(jsonSerializer);

            var mvcCoreBuilder = builder.Services
               .AddLogging()
               .AddMvcCore();


            //mvcCoreBuilder.AddMvcOptions(o =>
            //{
            //    o.OutputFormatters.Clear();
            //    o.OutputFormatters.Add(new JsonOutputFormatter(jsonSerializer));
            //    o.InputFormatters.Clear();
            //    o.InputFormatters.Add(new JsonInputFormatter(jsonSerializer));
            //});

            configureMvc?.Invoke(mvcCoreBuilder);

            return builder;
        }

        public static IHoneyCombBuilder AddErrorHandler<T>(this IHoneyCombBuilder builder) where T : class, IExceptionToResponseMapper
        {
            builder.Services.AddTransient<ErrorHandlerMiddleware>();
            builder.Services.AddTransient<IExceptionToResponseMapper, T>();
            return builder;
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            builder.ApplicationServices.GetRequiredService<IExceptionToResponseMapper>();
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder appBuilder, Action<IEndpointBuilder> builder)
        {
            appBuilder.UseRouting();
            appBuilder.UseEndpoints(router => builder?.Invoke(new EndpointBuilder(router)));
            return appBuilder;
        }

        public static async Task<T> ReadQueryAsync<T>(this HttpContext context) where T : class
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
            var serializer = context.RequestServices.GetRequiredService<IJsonSerializer>();
            if (values is null)
            {
                return serializer.Deserialize<T>(EmptyJsonObject);
            }

            var serialized = serializer.Serialize(values.ToDictionary(k => k.Key, k => k.Value))
                .Replace("\\\"", "\"")
                .Replace("\"{", "{")
                .Replace("}\"", "}")
                .Replace("\"[", "[")
                .Replace("]\"", "]");

            var payload = serializer.Deserialize<T>(serialized);
            var results = new List<ValidationResult>();
            if (payload != null && Validator.TryValidateObject(payload, new ValidationContext(payload), results))
            {
                return payload;
            }

            context.Response.StatusCode = 400;
            await context.Response.WriteJsonAsync(results);

            return default;
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
                T payload = default;
                var request = httpContext.Request;
                var serializer = httpContext.RequestServices.GetRequiredService<IJsonSerializer>();
                var data = await request.Body.GetAsStringAsync();
                if (string.IsNullOrWhiteSpace(data))
                    payload = serializer.Deserialize<T>(EmptyJsonObject);

                if (payload == null)
                    payload = serializer.Deserialize<T>(data);
                //payload = await httpContext.RequestServices.GetRequiredService<IJsonSerializer>().DeserializeAsync<T>(request.Body);

                if (_bindRequestFromRoute && HasRouteData(request))
                {
                    //Pobiera parametry z URL
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

                        var fieldValue = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(value.ToString());
                        field.SetValue(payload, fieldValue);
                    }
                }

                var results = new List<ValidationResult>();
                if (payload != null && Validator.TryValidateObject(payload, new ValidationContext(payload), results))
                {
                    return payload;
                }

                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteJsonAsync(results);

                return default;
            }
            catch (Exception)
            {
                throw;

                //httpContext.RequestServices.GetRequiredService<>

                //httpContext.Response.StatusCode = 400;
                //await httpContext.Response.Body.WriteAsync(InvalidJsonRequestBytes, 0, InvalidJsonRequestBytes.Length);

                //return default;
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

        public static async Task WriteJsonAsync<T>(this HttpResponse response, T value)
        {
            response.ContentType = JsonContentType;
            var data = JsonConvert.SerializeObject(value);
            await response.WriteAsync(data);
            //response.Body = data.GetAsStream();

            //var serializer = response.HttpContext.RequestServices.GetRequiredService<IJsonSerializer>();
            //await serializer.SerializeAsync(response.Body, value);
        }

        public static Task Ok(this HttpResponse response, object data = null)
        {
            response.StatusCode = 200;
            return data is null ? Task.CompletedTask : response.WriteJsonAsync(data);

        }

        public static Task Created(this HttpResponse response, string location = null, object data = null)
        {
            response.StatusCode = 201;
            if (string.IsNullOrWhiteSpace(location))
                return Task.CompletedTask;

            if (!response.Headers.ContainsKey(LocationHeader))
                response.Headers.Add(LocationHeader, location);

            return data is null ? Task.CompletedTask : response.WriteJsonAsync(data);

        }

        public static Task BadRequest(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Task.CompletedTask;
        }

        public static Task Unauthorized(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public static Task Forbidden(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        public static Task NotFound(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        public static Task InternalServerError(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Task.CompletedTask;
        }

        public static Task Accepted(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.Accepted;
            return Task.CompletedTask;
        }

        public static Task NoContent(this HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NoContent;
            return Task.CompletedTask;
        }

        public static Task MovedPermanently(this HttpResponse response, string url)
        {
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            if (!response.Headers.ContainsKey(LocationHeader))
            {
                response.Headers.Add(LocationHeader, url);
            }

            return Task.CompletedTask;
        }

        public static Task Redirect(this HttpResponse response, string url)
        {
            response.StatusCode = (int)HttpStatusCode.PermanentRedirect;
            if (!response.Headers.ContainsKey(LocationHeader))
            {
                response.Headers.Add(LocationHeader, url);
            }

            return Task.CompletedTask;
        }



        private static bool HasQueryString(this HttpRequest request)
           => request.Query.Any();

        private static bool HasRouteData(this HttpRequest request)
           => request.HttpContext.GetRouteData().Values.Any();



    }
}
