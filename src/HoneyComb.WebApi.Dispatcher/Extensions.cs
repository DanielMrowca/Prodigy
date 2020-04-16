using HoneyComb.WebApi.Dispatcher.Builders;
using HoneyComb.WebApi.Dispatcher.Dispatchers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.Dispatcher
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddWebApiDispatcher(this IHoneyCombBuilder builder)
        {
            builder.AddWebApi();

            builder.Services.Scan(s => s
               .FromExecutingAssembly()
               .FromCallingAssembly()
               .FromApplicationDependencies()
               .AddClasses(c => c.AssignableTo(typeof(IHttpRequestHandler<>)))
               .AddClasses(c => c.AssignableTo(typeof(IHttpRequestHandler<,>)))
               .AsImplementedInterfaces()
               .WithTransientLifetime());

            builder.Services.AddSingleton<IHttpRequestDispatcher, HttpRequestDispatcher>();
            return builder;
        }
       
        public static IApplicationBuilder UseEndpointDispatcher(this IApplicationBuilder app,
            Action<IEndpointDispatcherBuilder> builder, bool useAuthorization = false,
            Action<IApplicationBuilder> middleware = null)
        {
            app.UseRouting();
            if (useAuthorization)
                app.UseAuthorization();

            middleware?.Invoke(app);
            app.UseEndpoints(router => builder?.Invoke(new EndpointDispatcherBuilder(new EndpointBuilder(router))));

            return app;
        }
    }
}
