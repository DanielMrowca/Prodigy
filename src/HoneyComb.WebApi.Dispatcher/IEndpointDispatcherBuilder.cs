using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HoneyComb.WebApi.Dispatcher
{
    public interface IEndpointDispatcherBuilder
    {
        IEndpointDispatcherBuilder Get(string path, Func<HttpContext, Task> context = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies);

        IEndpointDispatcherBuilder Get<TRequest, TResult>(string path,
            Func<TRequest, HttpContext, Task> beforeDispatch = null,
            Func<TRequest, TResult, HttpContext, Task> afterDispatch = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where TRequest : class, IHttpRequest<TResult>;

        IEndpointDispatcherBuilder Post(string path, Func<HttpContext, Task> context = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies);

        IEndpointDispatcherBuilder Post<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, IHttpRequest;

        IEndpointDispatcherBuilder Post<T, TResult>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, TResult, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, IHttpRequest<TResult>;

        IEndpointDispatcherBuilder Put(string path, Func<HttpContext, Task> context = null,
          Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
          params string[] policies);

        IEndpointDispatcherBuilder Put<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, IHttpRequest;

        IEndpointDispatcherBuilder Put<T, TResult>(string path,
          Func<T, HttpContext, Task> beforeDispatch = null,
          Func<T, TResult, HttpContext, Task> afterDispatch = null,
          Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
          params string[] policies) where T : class, IHttpRequest<TResult>;

        IEndpointDispatcherBuilder Delete(string path, Func<HttpContext, Task> context = null,
         Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
         params string[] policies);

        IEndpointDispatcherBuilder Delete<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, IHttpRequest;

        IEndpointDispatcherBuilder Delete<T, TResult>(string path,
          Func<T, HttpContext, Task> beforeDispatch = null,
          Func<T, TResult, HttpContext, Task> afterDispatch = null,
          Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
          params string[] policies) where T : class, IHttpRequest<TResult>;
    }
}
