using System;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.Collaborators.ResponseHandlers
{
    public class DefaultResponseHandler : IResponseHandler
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DefaultResponseHandler(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<T> ReadContent<T>(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var stringContent = await response.Content.ReadAsStringAsync();

            return string.IsNullOrWhiteSpace(stringContent) ?
                default(T) :
                _jsonSerializer.Deserialize<T>(stringContent);
        }
    }
}