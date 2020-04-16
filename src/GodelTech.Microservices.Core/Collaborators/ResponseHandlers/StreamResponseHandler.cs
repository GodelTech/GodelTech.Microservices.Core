using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Collaborators.ResponseHandlers
{
    public class StreamResponseHandler : IResponseHandler
    {
        public async Task<T> ReadContent<T>(HttpResponseMessage response)
        {
            if (typeof(T) != typeof(Stream))
                throw new InvalidOperationException(nameof(StreamResponseHandler) + " supports only Stream result type");

            return (T)(object)(await response.Content.ReadAsStreamAsync());
        }
    }
}