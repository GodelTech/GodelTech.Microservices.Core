using System;
using System.IO;
using System.Net.Http;
using GodelTech.Microservices.Core.Collaborators.ResponseHandlers;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ResponseHandlerFactory : IResponseHandlerFactory
    {
        private readonly IJsonSerializer _jsonSerializer;

        public ResponseHandlerFactory(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public IResponseHandler Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(string))
                return new StringResponseHandler();

            if (type == typeof(Stream))
                return new StreamResponseHandler();

            if (type == typeof(byte[]))
                return new ByteResponseHandler();

            if (type == typeof(HttpResponseMessage))
                return new HttpResponseMessageResponseHandler();

            return new DefaultResponseHandler(_jsonSerializer);
        }
    }
}