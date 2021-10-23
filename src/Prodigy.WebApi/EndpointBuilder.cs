using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Prodigy.WebApi.ModelBinding;

namespace Prodigy.WebApi
{
    public class EndpointBuilder : IEndpointBuilder
    {
        private readonly IEndpointRouteBuilder _routeBuilder;
        private readonly IModelBinderProvider _modelBinderProvider;

        public EndpointBuilder(IEndpointRouteBuilder routeBuilder, IModelBinderProvider modelBinderProvider)
        {
            _routeBuilder = routeBuilder;
            _modelBinderProvider = modelBinderProvider ?? throw new ArgumentNullException(nameof(modelBinderProvider));
        }

        public IEndpointBuilder Get(string path, Func<HttpContext, Task> context = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies)
        {
            var builder = _routeBuilder.MapGet(path, ctx => context?.Invoke(ctx));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Get<T>(string path, Func<T, HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies)
            where T : class
        {
            var builder = _routeBuilder.MapGet(path, ctx => BuildQueryContext(ctx, context));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Get<TRequest, TResult>(string path, Func<TRequest, HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where TRequest : class
        {
            var builder = _routeBuilder.MapGet(path, ctx => BuildQueryContext(ctx, context));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Post(string path, Func<HttpContext, Task> context = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies)
        {
            var builder = _routeBuilder.MapPost(path, ctx => context?.Invoke(ctx));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Post<T>(string path, Func<T, HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where T : class
        {
            var builder = _routeBuilder.MapPost(path, ctx => BuildRequestContext(ctx, context));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Put(string path, Func<HttpContext, Task> context = null,
          Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
          params string[] policies)
        {
            var builder = _routeBuilder.MapPut(path, ctx => context?.Invoke(ctx));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Put<T>(string path, Func<T, HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where T : class
        {
            var builder = _routeBuilder.MapPut(path, ctx => BuildRequestContext(ctx, context));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Delete(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies)
        {
            var builder = _routeBuilder.MapDelete(path, ctx => context?.Invoke(ctx));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }

        public IEndpointBuilder Delete<T>(string path, Func<T, HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where T : class
        {
            var builder = _routeBuilder.MapDelete(path, ctx => BuildQueryContext(ctx, context));
            endpoint?.Invoke(builder);
            ApplyAuthRolesAndPolicies(builder, auth, roles, policies);

            return this;
        }


        private static void ApplyAuthRolesAndPolicies(IEndpointConventionBuilder builder, bool auth, string roles,
            params string[] policies)
        {
            if (policies is { } && policies.Any())
            {
                builder.RequireAuthorization(policies);
                return;
            }

            var hasRoles = !string.IsNullOrWhiteSpace(roles);
            var authorize = new AuthorizeAttribute();
            if (hasRoles)
            {
                authorize.Roles = roles;
            }

            if (auth || hasRoles)
            {
                builder.RequireAuthorization(authorize);
            }
        }

        private async Task BuildRequestContext<T>(HttpContext httpContext, Func<T, HttpContext, Task> context = null)
            where T : class
        {
            var modelBinder = _modelBinderProvider.GetModelBinder(httpContext);
            var model = await modelBinder.BindModelAsync<T>(httpContext);
            
            if (model is null || context is null)
                return;

            await context.Invoke(model, httpContext);
        }

        private static async Task BuildQueryContext<T>(HttpContext httpContext, Func<T, HttpContext, Task> context = null)
            where T : class
        {
            var request = await httpContext.ReadQueryAsync<T>();
            if (request is null || context is null)
                return;

            await context.Invoke(request, httpContext);
        }



    }
}
