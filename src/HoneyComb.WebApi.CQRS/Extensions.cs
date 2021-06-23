using HoneyComb.WebApi.ContentResults;
using HoneyComb.WebApi.CQRS.Builders;
using HoneyComb.WebApi.ModelBinding;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.CQRS
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
