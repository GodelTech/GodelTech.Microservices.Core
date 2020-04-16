using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Collaborators.ResponseHandlers
{
    public class ByteResponseHandler : IResponseHandler
    {
        public async Task<T> ReadContent<T>(HttpResponseMessage response)
        {
            if (typeof(T) != typeof(Stream))
                throw new InvalidOperationException(nameof(ByteResponseHandler) + " supports only byte array result type");

            return (T)(object)(await response.Content.ReadAsByteArrayAsync());
        }
    }
}