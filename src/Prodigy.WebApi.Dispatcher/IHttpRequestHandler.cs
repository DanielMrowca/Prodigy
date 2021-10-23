using System.Threading.Tasks;

namespace Prodigy.WebApi.Dispatcher
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
