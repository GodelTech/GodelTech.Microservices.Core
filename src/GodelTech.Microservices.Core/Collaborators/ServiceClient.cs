using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Exceptions;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ServiceClient : IServiceClient
    {
        private readonly HttpClient _client;
        private readonly IRequestContentHandlerFactory _requestContentHandlerFactory;
        private readonly IResponseHandlerFactory _responseHandlerFactory;

        public bool ReturnDefaultOn404 { get; set; }

        public ServiceClient(
            HttpClient client,
            IRequestContentHandlerFactory requestContentHandlerFactory,
            IResponseHandlerFactory responseHandlerFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestContentHandlerFactory = requestContentHandlerFactory ?? throw new ArgumentNullException(nameof(requestContentHandlerFactory));
            _responseHandlerFactory = responseHandlerFactory ?? throw new ArgumentNullException(nameof(responseHandlerFactory));
        }

        public Task<TResult> HeadAsync<TResult>(string url)
            where TResult : class
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            return ProcessRequestAsync<TResult>(
                CreateRequest(HttpMethod.Head, url, null));
        }

        public Task<TResult> GetAsync<TResult>(string url)
            where TResult : class
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            return ProcessRequestAsync<TResult>(
                CreateRequest(HttpMethod.Get, url, null));
        }

        public Task<TResult> PostAsync<TResult>(string url, object body)
            where TResult : class
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            return ProcessRequestAsync<TResult>(
                CreateRequest(HttpMethod.Post, url, body));
        }

        public Task<TResult> DeleteAsync<TResult>(string url, object body = null)
            where TResult : class
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            return ProcessRequestAsync<TResult>(
                CreateRequest(HttpMethod.Delete, url, body));
        }

        public Task<TResult> PutAsync<TResult>(string url, object body = null)
            where TResult : class
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            return ProcessRequestAsync<TResult>(
                CreateRequest(HttpMethod.Put, url, body));
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object body) 
        {
            return new HttpRequestMessage(method, url)
            {
                Content = CreateContent(body)
            };
        }

        private HttpContent CreateContent(object body)
        {
            return body == null ? 
                null : 
                _requestContentHandlerFactory.Create(body.GetType()).Create(body);
        }

        private async Task<T> ProcessRequestAsync<T>(HttpRequestMessage request) where T : class
        {
            var result = await _client.SendAsync(request);

            if (result.StatusCode == HttpStatusCode.NotFound && ReturnDefaultOn404)
                return default(T);

            if (!result.IsSuccessStatusCode)
            {
                throw new CollaborationException($"Failed to process request. StatusCode={result.StatusCode}, Reason={result.ReasonPhrase}, Method={request.Method}, Url={request.RequestUri}")
                {
                    StatusCode = (int)result.StatusCode,
                    ReasonPhrase = result.ReasonPhrase
                };
            }

            var handler = _responseHandlerFactory.Create(typeof(T));
            return await handler.ReadContent<T>(result);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
