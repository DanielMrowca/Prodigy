using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.Dispatcher
{
    public interface IHttpRequest
    {
    }

    public interface IHttpRequest<T> : IHttpRequest
    {

    }
}
