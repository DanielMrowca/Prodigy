using HoneyComb.HTTP.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.HTTP
{
    public class HoneyCombHttpClient : IHttpClient
    {
        private const string ApplicationJsonContentType = "application/json";
        private static readonly StringContent EmptyJson = new StringContent("{}", Encoding.UTF8, ApplicationJsonContentType);
        private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly HttpClient _httpClient;
        private readonly HttpClientOptions _options;

        public HoneyCombHttpClient(HttpClient httpClient, HttpClientOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
            => SendAsync(uri, Method.Get);

        public Task<T> GetAsync<T>(string uri)
            => SendAsync<T>(uri, Method.Get);

        public Task<HttpResult<T>> GetResultAsync<T>(string uri)
            => SendResultAsync<T>(uri, Method.Get);

        public Task<HttpResponseMessage> PostAsync(string uri, object data = null)
            => SendAsync(uri, Method.Post, data);

        public Task<T> PostAsync<T>(string uri, object data = null)
            => SendAsync<T>(uri, Method.Post, data);

        public Task<HttpResult<T>> PostResultAsync<T>(string uri, object data = null)
            => SendResultAsync<T>(uri, Method.Post, data);
        public Task<HttpResponseMessage> PutAsync(string uri, object data = null)
            => SendAsync(uri, Method.Put, data);

        public Task<T> PutAsync<T>(string uri, object data = null)
            => SendAsync<T>(uri, Method.Put, data);

        public Task<HttpResult<T>> PutResultAsync<T>(string uri, object data = null)
            => SendResultAsync<T>(uri, Method.Put, data);

        public Task<HttpResponseMessage> PatchAsync(string uri, object data = null)
            => SendAsync(uri, Method.Patch, data);

        public Task<T> PatchAsync<T>(string uri, object data = null)
            => SendAsync<T>(uri, Method.Patch, data);

        public Task<HttpResult<T>> PatchResultAsync<T>(string uri, object data = null)
            => SendResultAsync<T>(uri, Method.Patch, data);

        public Task<HttpResponseMessage> DeleteAsync(string uri)
            => SendAsync(uri, Method.Delete);

        public Task<T> DeleteAsync<T>(string uri)
            => SendAsync<T>(uri, Method.Delete);

        public Task<HttpResult<T>> DeleteResultAsync<T>(string uri)
            => SendResultAsync<T>(uri, Method.Delete);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
                .ExecuteAsync(() => _httpClient.SendAsync(request));
        }

        public Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            return Policy.Handle<Exception>()
               .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
               .ExecuteAsync(async () =>
               {
                   var response = await _httpClient.SendAsync(request);

                   if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                       await HandleResponseError(response);

                   if (!response.IsSuccessStatusCode)
                       return default;

                   var stringContent = await response.Content.ReadAsStringAsync();
                   if (string.IsNullOrWhiteSpace(stringContent))
                       return default;

                   return JsonConvert.DeserializeObject<T>(stringContent, JsonSerializerSettings);
               });
        }

        public Task<HttpResult<T>> SendResultAsync<T>(HttpRequestMessage request)
        {
            return Policy.Handle<Exception>()
              .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
              .ExecuteAsync(async () =>
              {
                  var response = await _httpClient.SendAsync(request);

                  if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                      await HandleResponseError(response);

                  if (!response.IsSuccessStatusCode)
                      return new HttpResult<T>(default, response);

                  var stringContent = await response.Content.ReadAsStringAsync();
                  if (string.IsNullOrWhiteSpace(stringContent))
                      return new HttpResult<T>(default, response);

                  var result = JsonConvert.DeserializeObject<T>(stringContent, JsonSerializerSettings);
                  return new HttpResult<T>(result, response);
              });
        }

        public void SetHeaders(IDictionary<string, string> headers)
        {
            if (headers is null)
                return;

            foreach (var (key, value) in headers)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }
        }
        public void SetHeaders(Action<HttpRequestHeaders> headers) => headers?.Invoke(_httpClient.DefaultRequestHeaders);
        public void SetAuthorizationHeader(string scheme, string parameter)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }

        protected virtual Task<HttpResponseMessage> SendAsync(string uri, Method method, object data = null)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
                .ExecuteAsync(async () =>
                {
                    var requestUri = uri.StartsWith("http") ? uri : $"http://{uri}";
                    var response = await GetResponseAsync(uri, method, data);
                    if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                        await HandleResponseError(response);

                    return response;
                });
        }

        protected virtual async Task<T> SendAsync<T>(string uri, Method method, object data = null)
        {
            var response = await SendAsync(uri, method, data);
            if (!response.IsSuccessStatusCode)
                return default;

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return default;

            return JsonConvert.DeserializeObject<T>(stringContent, JsonSerializerSettings);
        }

        protected virtual async Task<HttpResult<T>> SendResultAsync<T>(string uri, Method method, object data = null)
        {
            var response = await SendAsync(uri, method, data);
            if (!response.IsSuccessStatusCode)
                return new HttpResult<T>(default, response);

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return new HttpResult<T>(default, response);

            var result = JsonConvert.DeserializeObject<T>(stringContent, JsonSerializerSettings);
            return new HttpResult<T>(result, response);
        }

        protected virtual Task<HttpResponseMessage> GetResponseAsync(string uri, Method method, object data = null)
        {
            switch (method)
            {
                case Method.Get:
                    return _httpClient.GetAsync(uri);
                case Method.Post:
                    return _httpClient.PostAsync(uri, GetJsonData(data));
                case Method.Put:
                    return _httpClient.PutAsync(uri, GetJsonData(data));
                case Method.Patch:
                    return _httpClient.PatchAsync(uri, GetJsonData(data));
                case Method.Delete:
                    return _httpClient.DeleteAsync(uri);
                default:
                    throw new InvalidOperationException($"Http method: {method} is not supported");

            }
        }

        protected virtual async Task HandleResponseError(HttpResponseMessage errorResponse)
        {
            var stringResponse = await errorResponse.Content.ReadAsStringAsync();
            var exReponse = JsonConvert.DeserializeObject(stringResponse);
            throw new HttpResponseException(errorResponse, stringResponse, errorResponse.ReasonPhrase);
        }

        protected static StringContent GetJsonData(object data)
        {
            return data == null ? EmptyJson : new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, ApplicationJsonContentType);
        }

        protected enum Method
        {
            Get,
            Post,
            Put,
            Patch,
            Delete
        }
    }
}
