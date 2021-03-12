using HoneyComb.HTTP.Exceptions;
using HoneyComb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        protected readonly HttpClient _httpClient;
        private readonly HttpClientOptions _options;
        private readonly IEnumerable<JsonConverter> _jsonConverters;
        //private readonly AppSettings _appSettings;

        public HoneyCombHttpClient(HttpClient httpClient, HttpClientOptions options,
            IEnumerable<JsonConverter> jsonConverters)//, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _options = options;
            _jsonConverters = jsonConverters;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = jsonConverters?.ToList()
            };
            //_appSettings = appSettings;

            //var appContext = $"{_appSettings.Name} {_appSettings.Version} ({_appSettings.VersionNumber})";
            //_httpClient.DefaultRequestHeaders.Add("AppContext", appContext);
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
            => SendAsync(uri, Method.Get);

        public Task<T> GetAsync<T>(string uri)
            => SendAsync<T>(uri, Method.Get);

        public Task<HttpResult<T>> GetResultAsync<T>(string uri)
            => SendResultAsync<T>(uri, Method.Get);

        public Task<HttpResponseMessage> PostAsync(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync(uri, Method.Post, data, compression);

        public Task<T> PostAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync<T>(uri, Method.Post, data, compression);

        public Task<HttpResult<T>> PostResultAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendResultAsync<T>(uri, Method.Post, data, compression);
        public Task<HttpResponseMessage> PutAsync(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync(uri, Method.Put, data, compression);

        public Task<T> PutAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync<T>(uri, Method.Put, data, compression);

        public Task<HttpResult<T>> PutResultAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendResultAsync<T>(uri, Method.Put, data, compression);

        public Task<HttpResponseMessage> PatchAsync(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync(uri, Method.Patch, data, compression);

        public Task<T> PatchAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendAsync<T>(uri, Method.Patch, data, compression);

        public Task<HttpResult<T>> PatchResultAsync<T>(string uri, object data = null, CompressionMethod? compression = null)
            => SendResultAsync<T>(uri, Method.Patch, data, compression);

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
                   return await HandleSendAsync<T>(request);
               });
        }

        protected virtual async Task<T> HandleSendAsync<T>(HttpRequestMessage request, bool isRetryAfterUnauthorized = false)
        {
            var response = await _httpClient.SendAsync(request);

            // Retry only once on unauthorized request
            if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetryAfterUnauthorized)
            {
                var shouldRetryWithoutThrowException = await HandleUnauthorizedRequest(response);
                if (shouldRetryWithoutThrowException)
                    return await HandleSendAsync<T>(request, true);
            }

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                await HandleResponseError(response);

            if (!response.IsSuccessStatusCode)
                return default;

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return default;

            return JsonConvert.DeserializeObject<T>(stringContent, _jsonSerializerSettings);
        }

        public Task<HttpResult<T>> SendResultAsync<T>(HttpRequestMessage request)
        {
            return Policy.Handle<Exception>()
              .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
              .ExecuteAsync(async () =>
              {
                  return await HandleSendResultAsync<T>(request);
              });
        }

        protected async Task<HttpResult<T>> HandleSendResultAsync<T>(HttpRequestMessage request, bool isRetryAfterUnauthorized = false)
        {
            await BeforeSendRequestAsync(_httpClient);
            var response = await _httpClient.SendAsync(request);

            // Retry only once on unauthorized request
            if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetryAfterUnauthorized)
            {
                var shouldRetryWithoutThrowException = await HandleUnauthorizedRequest(response);
                if (shouldRetryWithoutThrowException)
                    return await HandleSendResultAsync<T>(request, true);
            }

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                await HandleResponseError(response);

            if (!response.IsSuccessStatusCode)
                return new HttpResult<T>(default, response);

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return new HttpResult<T>(default, response);

            var result = JsonConvert.DeserializeObject<T>(stringContent, _jsonSerializerSettings);
            return new HttpResult<T>(result, response);
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

        protected virtual Task<HttpResponseMessage> SendAsync(string uri, Method method, object data = null, CompressionMethod? compression = null)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(3 * r))
                .ExecuteAsync(async () =>
                {
                    return await HandleSendAsync(uri, method, data, compression);
                });
        }

        protected async Task<HttpResponseMessage> HandleSendAsync(string uri, Method method, object data = null, CompressionMethod? compression = null,
            bool isRetryAfterUnauthorized = false)
        {
            await BeforeSendRequestAsync(_httpClient);

            var requestUri = uri.StartsWith("http") ? uri : $"http://{uri}";
            var response = await GetResponseAsync(uri, method, data, GetCompressionMethod(compression));

            // Retry only once on unauthorized request
            if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetryAfterUnauthorized)
            {
                var shouldRetryWithoutThrowException = await HandleUnauthorizedRequest(response);
                if (shouldRetryWithoutThrowException)
                    return await HandleSendAsync(uri, method, data, compression, true);
            }

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                await HandleResponseError(response);

            return response;
        }

        protected virtual async Task<T> SendAsync<T>(string uri, Method method, object data = null, CompressionMethod? compression = null)
        {
            var response = await SendAsync(uri, method, data, compression);
            if (!response.IsSuccessStatusCode)
                return default;

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return default;

            return JsonConvert.DeserializeObject<T>(stringContent, _jsonSerializerSettings);
        }

        protected virtual async Task<HttpResult<T>> SendResultAsync<T>(string uri, Method method, object data = null, CompressionMethod? compression = null)
        {
            var response = await SendAsync(uri, method, data, compression);
            if (!response.IsSuccessStatusCode)
                return new HttpResult<T>(default, response);

            var stringContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(stringContent))
                return new HttpResult<T>(default, response);

            var result = JsonConvert.DeserializeObject<T>(stringContent, _jsonSerializerSettings);
            return new HttpResult<T>(result, response);
        }

        protected virtual Task<HttpResponseMessage> GetResponseAsync(string uri, Method method, object data = null, CompressionMethod? compression = null)
        {
            switch (method)
            {
                case Method.Get:
                    return _httpClient.GetAsync(uri);
                case Method.Post:
                    return _httpClient.PostAsync(uri, GetJsonData(data, compression));
                case Method.Put:
                    return _httpClient.PutAsync(uri, GetJsonData(data, compression));
                case Method.Patch:
                    return _httpClient.PatchAsync(uri, GetJsonData(data, compression));
                case Method.Delete:
                    return _httpClient.DeleteAsync(uri);
                default:
                    throw new InvalidOperationException($"Http method: {method} is not supported");
            }
        }

        protected virtual async Task HandleResponseError(HttpResponseMessage errorResponse)
        {
            var stringResponse = await errorResponse.Content.ReadAsStringAsync();
            throw new HttpResponseException(errorResponse, stringResponse, errorResponse.ReasonPhrase);
        }

        /// <summary>
        ///     Method for implementing custom action on unathorized request.
        ///     Best place for handle <i>REFRESH TOKEN</i> and return TRUE after success.
        /// </summary>
        /// <remarks>
        ///     If this method return <i>TRUE</i> executing of current http request will be retry.
        /// </remarks>
        /// <param name="errorResponse"></param>
        /// <returns></returns>
        protected virtual Task<bool> HandleUnauthorizedRequest(HttpResponseMessage errorResponse)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        ///     This method is executing before every http request. You can implement here setting access token or 
        ///     other extra data to httpClient
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        protected virtual Task BeforeSendRequestAsync(HttpClient httpClient)
        {
            return Task.CompletedTask;
        }

        protected HttpContent GetJsonData(object data, CompressionMethod? compression = null)
        {
            if (data is null)
                return EmptyJson;

            var json = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            var stringContent = new StringContent(json, Encoding.UTF8, ApplicationJsonContentType);
            if (compression is null)
                return stringContent;

            return new CompressedHttpContent(stringContent, compression.Value);
        }

        private CompressionMethod? GetCompressionMethod(CompressionMethod? requestCompression)
        {
            // If request compression is null check global configuration
            if (requestCompression is null)
            {
                if (_options.Compression.IsEnabled)
                    return _options.Compression.Method.ToEnum(CompressionMethod.GZip);
            }

            return requestCompression;
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
