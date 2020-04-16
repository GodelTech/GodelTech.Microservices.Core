using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.Collaborators.Handlers
{
    public class BearerAccessTokenHandler : DelegatingHandler
    {
        private readonly IBearerTokenStorage _tokenStorage;
        private readonly bool _excludeAccessToken;

        public BearerAccessTokenHandler(IBearerTokenStorage tokenStorage, bool excludeAccessToken)
        {
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _excludeAccessToken = excludeAccessToken;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_excludeAccessToken)
                return base.SendAsync(request, cancellationToken);

            if (request.Headers.Authorization != null)
                return base.SendAsync(request, cancellationToken);

            var token = _tokenStorage.GetAccessToken();

            if (string.IsNullOrWhiteSpace(token))
                return base.SendAsync(request, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}