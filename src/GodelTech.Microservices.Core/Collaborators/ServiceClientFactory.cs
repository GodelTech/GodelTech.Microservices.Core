using System;
using System.Net.Http;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ServiceClientFactory : IServiceClientFactory
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRequestContentHandlerFactory _requestContentHandlerFactory;
        private readonly IResponseHandlerFactory _responseHandlerFactory;
        private readonly IServiceRegistry _serviceRegistry;

        public ServiceClientFactory(
            IHttpClientFactory clientFactory,
            IRequestContentHandlerFactory requestContentHandlerFactory,
            IResponseHandlerFactory responseHandlerFactory,
            IServiceRegistry serviceRegistry)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _requestContentHandlerFactory = requestContentHandlerFactory ?? throw new ArgumentNullException(nameof(requestContentHandlerFactory));
            _responseHandlerFactory = responseHandlerFactory ?? throw new ArgumentNullException(nameof(responseHandlerFactory));
            _serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
        }

        public IServiceClient Create(string serviceName, bool returnDefaultOn404 = false)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceName));

            var config = _serviceRegistry.GetConfig(serviceName);
            if (config == null)
                throw new ArgumentException($"Configuration for service \"{serviceName}\" was not found", nameof(serviceName));

            return new ServiceClient(
                _clientFactory.CreateClient(serviceName.ToLowerInvariant()),
                _requestContentHandlerFactory,
                _responseHandlerFactory)
            {
                ReturnDefaultOn404 = returnDefaultOn404
            };
        }
    }
}