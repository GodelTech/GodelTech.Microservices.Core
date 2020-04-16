using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Collaborators.Handlers
{
    public sealed class ClientCredentialsAuthTokenHandler : DelegatingHandler
    {
        private readonly ILogger<ClientCredentialsAuthTokenHandler> _logger;
        private readonly IIdentityService _identityService;
        private readonly IBearerTokenStorage _bearerTokenStorage;

        public ClientCredentialsAuthTokenHandler(
            ILogger<ClientCredentialsAuthTokenHandler> logger,
            IIdentityService identityService,
            IBearerTokenStorage bearerTokenStorage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _bearerTokenStorage = bearerTokenStorage ?? throw new ArgumentNullException(nameof(bearerTokenStorage));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers.Authorization != null)
            {
                _logger.LogDebug("Authorization header for the request already exists, skipping token retrieval.");
                return base.SendAsync(request, cancellationToken);
            }

            return DoSendAsync(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> DoSendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _identityService.GetClientCredentialsTokenAsync();

            using (_bearerTokenStorage.SetAccessToken(token))
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}