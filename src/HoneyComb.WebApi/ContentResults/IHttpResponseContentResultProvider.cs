using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.ContentResults
{
    public interface IHttpResponseContentResultProvider
    {
        IHttpResponseContentResult GetResponseContentResult<TResult>(TResult result);
    }
}
