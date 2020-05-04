using System;
using System.Net.Http;
using System.Net;

namespace HoneyComb.HTTP.Exceptions
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
