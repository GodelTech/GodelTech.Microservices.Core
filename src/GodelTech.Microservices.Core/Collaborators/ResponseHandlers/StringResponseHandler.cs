using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Collaborators.ResponseHandlers
{
    public class StringResponseHandler : IResponseHandler
    {
        public async Task<T> ReadContent<T>(HttpResponseMessage response)
        {
            if (typeof(T) != typeof(string))
                throw new InvalidOperationException(nameof(StringResponseHandler) + " supports only string result type");

            return (T)(object)(await response.Content.ReadAsStringAsync());
        }
    }
}