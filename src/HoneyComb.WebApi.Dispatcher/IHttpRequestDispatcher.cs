using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.Dispatcher
{
    public interface IHttpRequestDispatcher
    {
        Task HandleRequestAsync<TRequest>(TRequest request) where TRequest : class, IHttpRequest;
        Task<TResult> HandleRequestAsync<TRequest, TResult>(TRequest query) where TRequest : class, IHttpRequest<TResult>;
    }
}
