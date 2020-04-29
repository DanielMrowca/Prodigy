using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;

namespace HoneyComb.HTTP.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public HttpResponseMessage Response { get; private set; }

        public HttpResponseException(HttpResponseMessage response)
            : base("Http request error")
        {
            Response = response;
            StatusCode = response.StatusCode;
        }
    }
}
