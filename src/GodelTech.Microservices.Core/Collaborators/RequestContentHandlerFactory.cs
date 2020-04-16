using System;
using System.Net.Http;
using GodelTech.Microservices.Core.Collaborators.RequestHandlers;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class RequestContentHandlerFactory : IRequestContentHandlerFactory
    {
        private readonly IJsonSerializer _jsonSerializer;

        public RequestContentHandlerFactory(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public IRequestContentHandler Create(Type type)
        {
            if (type == typeof(HttpContent))
                return new HttpRequestContentHandler();

            return new DefaultRequestContentHandler(_jsonSerializer);
        }
    }
}