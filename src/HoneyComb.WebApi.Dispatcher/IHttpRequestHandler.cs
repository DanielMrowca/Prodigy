using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.Dispatcher
{
    public interface IHttpRequestHandler<in TRequest, TResult>
    {
        Task<TResult> HandleAsync(TRequest request);
    }

    public interface IHttpRequestHandler<in TRequest>
    {
        Task HandleAsync(TRequest request);
    }
}
