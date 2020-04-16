using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.Security;
using GodelTech.Microservices.Core.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Collaborators.Handlers
{
    public sealed class TenantClientCredentialsAuthTokenHandler : DelegatingHandler
    {
        private readonly ILogger<TenantClientCredentialsAuthTokenHandler> _logger;
        private readonly ISecurityContext _securityContext;
        private readonly IIdentityService _identityService;
        private readonly IBearerTokenStorage _bearerTokenStorage;

        public TenantClientCredentialsAuthTokenHandler(
            ILogger<TenantClientCredentialsAuthTokenHandler> logger, 
            ISecurityContext securityContext,
            IIdentityService identityService, 
            IBearerTokenStorage bearerTokenStorage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _securityContext = securityContext ?? throw new ArgumentNullException(nameof(securityContext));
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
            int? tenantId = _securityContext.TenantId;

            if (!tenantId.HasValue)
            {
                _logger.LogError("Tenant information is not available. No token will be specified.");

                return await base.SendAsync(request, cancellationToken);
            }
            
            var token = await _identityService.GetTenantClientCredentialsTokenAsync(tenantId.Value);

            using (_bearerTokenStorage.SetAccessToken(token))
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}