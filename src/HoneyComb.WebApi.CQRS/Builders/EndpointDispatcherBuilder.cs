using HoneyComb.CQRS.Commands;
using HoneyComb.CQRS.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.CQRS.Builders
{
    public class EndpointDispatcherBuilder : IEndpointDispatcherBuilder
    {
        private readonly IEndpointBuilder _builder;

        public EndpointDispatcherBuilder(IEndpointBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public IEndpointDispatcherBuilder Get(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Get(path, context, endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Get<TQuery, TResult>(string path,
            Func<TQuery, HttpContext, Task> beforeDispatch,
            Func<TQuery, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint, bool auth,
            string roles, params string[] policies) where TQuery : class, IQuery<TResult>

        {
            _builder.Get<TQuery>(path, async (query, ctx) =>
             {
                 if (!(beforeDispatch is null))
                     await beforeDispatch?.Invoke(query, ctx);

                 var queryDispatcher = ctx.RequestServices.GetRequiredService<IQueryDispatcher>();
                 var result = await queryDispatcher.QueryAsync<TQuery, TResult>(query);

                 if (afterDispatch is null)
                 {
                     if (result is null)
                     {
                         ctx.Response.StatusCode = 404;
                         return;
                     }


                     await ctx.Response.WriteJsonAsync(result);
                     return;
                 }

                 await afterDispatch?.Invoke(query, result, ctx);

             }, endpoint, auth, roles, policies);

            return this;
        }

        public IEndpointDispatcherBuilder Post(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Post(path, context, endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Post<T>(string path,
            Func<T, HttpContext, Task> beforeDispatch,
            Func<T, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint, bool auth,
            string roles, params string[] policies) where T : class, ICommand
        {
            _builder.Post<T>(path, (cmd, ctx) => BuildCommandContext(cmd, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Post<T, TResult>(
            string path, Func<T, HttpContext, Task> beforeDispatch,
            Func<T, TResult, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint,
            bool returnResult,
            bool auth,
            string roles,
            params string[] policies) where T : class, ICommand<TResult>
        {
            _builder.Post<T>(path, (cmd, ctx) => BuildCommandContext<T, TResult>(cmd, ctx, returnResult, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Put(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Put(path, context, endpoint, auth, roles, policies);
            return this;
        }

        IEndpointDispatcherBuilder IEndpointDispatcherBuilder.Put<T>(string path,
            Func<T, HttpContext, Task> beforeDispatch,
            Func<T, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint, bool auth, string roles, params string[] policies)
        {
            _builder.Put<T>(path, (cmd, ctx) => BuildCommandContext(cmd, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        public IEndpointDispatcherBuilder Delete(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null, params string[] policies)
        {
            _builder.Delete(path, context, endpoint, auth, roles, policies);
            return this;
        }

        IEndpointDispatcherBuilder IEndpointDispatcherBuilder.Delete<T>(string path,
            Func<T, HttpContext, Task> beforeDispatch,
            Func<T, HttpContext, Task> afterDispatch,
            Action<IEndpointConventionBuilder> endpoint, bool auth, string roles, params string[] policies)
        {
            _builder.Delete<T>(path, (cmd, ctx) => BuildCommandContext(cmd, ctx, beforeDispatch, afterDispatch), endpoint, auth, roles, policies);
            return this;
        }

        private static async Task BuildCommandContext<T>(T command,
            HttpContext context,
            Func<T, HttpContext, Task> beforeDispatch = null,
            Func<T, HttpContext, Task> afterDispatch = null) where T : class, ICommand
        {
            if (!(beforeDispatch is null))
                await beforeDispatch?.Invoke(command, context);

            var dispatcher = context.RequestServices.GetRequiredService<ICommandDispatcher>();
            await dispatcher.SendAsync(command);

            if (afterDispatch is null)
            {
                context.Response.StatusCode = 200;
                return;
            }

            await afterDispatch?.Invoke(command, context);
        }


        private static async Task BuildCommandContext<T, TResult>(T command,
            HttpContext context,
            bool returnResult = true,
            Func<T, HttpContext, Task> beforeDispatch = null,
            Func<T, TResult, HttpContext, Task> afterDispatch = null) where T : class, ICommand<TResult>
        {
            if (!(beforeDispatch is null))
                await beforeDispatch?.Invoke(command, context);

            var dispatcher = context.RequestServices.GetRequiredService<ICommandDispatcher>();
            var result = await dispatcher.SendAsync(command);

            if (afterDispatch is null)
            {
                context.Response.StatusCode = 200;

                if (result != null && returnResult)
                {
                    await context.Response.WriteJsonAsync(result);
                    return;
                }
            }

            await afterDispatch?.Invoke(command, result, context);
        }


    }
}
