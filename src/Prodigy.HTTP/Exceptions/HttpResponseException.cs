using System;
using System.Net;
using System.Net.Http;

namespace Prodigy.HTTP.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public string RawResponse { get; private set; }

        public HttpResponseException(HttpResponseMessage response, string rawResponse)
            : base("Http request error")
        {
            Response = response;
            StatusCode = response.StatusCode;
            RawResponse = rawResponse;
        }

        public HttpResponseException(HttpResponseMessage response, string rawResponse, string message)
            : base(message)
        {
            Response = response;
            StatusCode = response.StatusCode;
            RawResponse = rawResponse;
        }
    }
}
