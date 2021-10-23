using System;
using Microsoft.AspNetCore.Builder;
using Prodigy.WebApi.ContentResults;
using Prodigy.WebApi.CQRS.Builders;
using Prodigy.WebApi.ModelBinding;

namespace Prodigy.WebApi.CQRS
{
    public static class Extensions
    {

        public static IApplicationBuilder UseEndpointDispatcher(this IApplicationBuilder app,
            Action<IEndpointDispatcherBuilder> builder, bool useAuthorization = false,
            Action<IApplicationBuilder> middleware = null)
        {
            app.UseRouting();
            if (useAuthorization)
                app.UseAuthorization();

            middleware?.Invoke(app);
            app.UseEndpoints(router => builder?.Invoke(new EndpointDispatcherBuilder(new EndpointBuilder(router, new DefaultModelBinderProvider()), new DefaultHttpResponseContentResultProvider())));

            return app;
        }
    }
}
