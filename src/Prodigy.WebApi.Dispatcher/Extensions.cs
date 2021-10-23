using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.WebApi.Dispatcher.Builders;
using Prodigy.WebApi.Dispatcher.Dispatchers;
using Prodigy.WebApi.ModelBinding;

namespace Prodigy.WebApi.Dispatcher
{
    public static class Extensions
    {
        public static IProdigyBuilder AddWebApiDispatcher(this IProdigyBuilder builder)
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
            app.UseEndpoints(router => builder?.Invoke(new EndpointDispatcherBuilder(new EndpointBuilder(router, new DefaultModelBinderProvider()))));

            return app;
        }
    }
}
