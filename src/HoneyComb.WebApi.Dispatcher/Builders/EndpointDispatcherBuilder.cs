using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.Dispatcher.Builders
{
    public class EndpointDispatcherBuilder : IEndpointDispatcherBuilder
    {
        private readonly IEndpointBuilder _builder;

        public EndpointDispatcherBuilder(IEndpointBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }
        public IEndpointDispatcherBuilder Get(string path,
            Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null,
            bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Get(path, context, endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Get<TRequest, TResult>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest<TResult>
        {
            _builder.Get<TRequest>(path, (request, ctx) => BuildHttpRequest(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Post(string path,
            Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null,
            bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Post(path, context, endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Post<TRequest>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest
        {
            _builder.Post<TRequest>(path, (request, ctx) => BuildHttpRequestWithoutResult(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Post<TRequest, TResult>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest<TResult>
        {
            _builder.Post<TRequest>(path, (request, ctx) => BuildHttpRequest(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Put(string path,
            Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null,
            bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Put(path, context, endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Put<TRequest>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest
        {
            _builder.Put<TRequest>(path, (request, ctx) => BuildHttpRequestWithoutResult(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Put<TRequest, TResult>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest<TResult>
        {
            _builder.Post<TRequest>(path, (request, ctx) => BuildHttpRequest(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }


        public IEndpointDispatcherBuilder Delete(string path,
            Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null,
            bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Delete(path, context, endpoint, auth, roles, policies);
            return this;
        }


        public IEndpointDispatcherBuilder Delete<TRequest>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest
        {
            _builder.Delete<TRequest>(path, (request, ctx) => BuildHttpRequestWithoutResult(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Delete<TRequest, TResult>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool auth, string roles, params string[] policies) where TRequest : class, IHttpRequest<TResult>
        {
            _builder.Delete<TRequest>(path, (request, ctx) => BuildHttpRequest(request, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }


        private static async Task BuildHttpRequestWithoutResult<TRequest>(TRequest request, HttpContext httpContext,
             Func<TRequest, HttpContext, Task> beforeDispatch,
             Func<TRequest, HttpContext, Task> afterDispatch) where TRequest : class, IHttpRequest
        {
            if (!(beforeDispatch is null))
                await beforeDispatch?.Invoke(request, httpContext);

            var dispatcher = httpContext.RequestServices.GetRequiredService<IHttpRequestDispatcher>();
            await dispatcher.HandleRequestAsync(request);

            if (afterDispatch is null)
            {
                httpContext.Response.StatusCode = 200;
                return;
            }

            await afterDispatch?.Invoke(request, httpContext);
        }

        private static async Task BuildHttpRequest<TRequest, TResult>(TRequest request, HttpContext httpContext,
            Func<TRequest, HttpContext, Task> beforeDispatch,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch) where TRequest : class, IHttpRequest<TResult>
        {
            if (!(beforeDispatch is null))
                await beforeDispatch?.Invoke(request, httpContext);

            var queryDispatcher = httpContext.RequestServices.GetRequiredService<IHttpRequestDispatcher>();
            var result = await queryDispatcher.HandleRequestAsync<TRequest, TResult>(request);

            if (afterDispatch is null)
            {
                if (result is null)
                {
                    httpContext.Response.StatusCode = 404;
                    return;
                }

                await httpContext.Response.WriteJsonAsync(result);
                return;
            }

            await afterDispatch?.Invoke(request, result, httpContext);
        }

    }
}
