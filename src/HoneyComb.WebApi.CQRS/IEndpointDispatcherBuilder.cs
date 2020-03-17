using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using HoneyComb.CQRS.Queries;
using HoneyComb.CQRS.Commands;

namespace HoneyComb.WebApi.CQRS
{
    public interface IEndpointDispatcherBuilder
    {
        IEndpointDispatcherBuilder Get(string path, Func<HttpContext, Task> context = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies);

        IEndpointDispatcherBuilder Get<TQuery, TResult>(string path,
            Func<TQuery, HttpContext, Task> beforeDispatch = null,
            Func<TQuery, TResult, HttpContext, Task> afterDispatch = null,
            Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
            params string[] policies) where TQuery : class, IQuery<TResult>;

        IEndpointDispatcherBuilder Post(string path, Func<HttpContext, Task> context = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies);

        IEndpointDispatcherBuilder Post<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, ICommand;

        IEndpointDispatcherBuilder Put(string path, Func<HttpContext, Task> context = null,
          Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
          params string[] policies);

        IEndpointDispatcherBuilder Put<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, ICommand;

        IEndpointDispatcherBuilder Delete(string path, Func<HttpContext, Task> context = null,
         Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
         params string[] policies);

        IEndpointDispatcherBuilder Delete<T>(string path,
           Func<T, HttpContext, Task> beforeDispatch = null,
           Func<T, HttpContext, Task> afterDispatch = null,
           Action<IEndpointConventionBuilder> endpoint = null, bool auth = false, string roles = null,
           params string[] policies) where T : class, ICommand;
    }
}
