using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.Dispatcher.Dispatchers
{
    public class HttpRequestDispatcher : IHttpRequestDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HttpRequestDispatcher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleRequestAsync<TRequest>(TRequest request) where TRequest : class, IHttpRequest
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                dynamic handler = scope.ServiceProvider.GetRequiredService<IHttpRequestHandler<TRequest>>();
                await handler.HandleAsync(request);
            }
        }

        public async Task<TResult> HandleRequestAsync<TRequest, TResult>(TRequest request) where TRequest : class, IHttpRequest<TResult>
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                dynamic handler = scope.ServiceProvider.GetRequiredService<IHttpRequestHandler<TRequest,TResult>>();
                return await handler.HandleAsync(request);
            }
        }
    }
}
