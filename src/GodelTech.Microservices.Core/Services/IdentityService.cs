using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Exceptions;
using IdentityModel;
using IdentityModel.Client;

namespace GodelTech.Microservices.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityConfiguration _identityConfiguration;
        private readonly IHttpClientFactory _clientFactory;

        public IdentityService(
            IIdentityConfiguration identityConfiguration,
            IHttpClientFactory clientFactory)
        {
            _identityConfiguration = identityConfiguration ?? throw new ArgumentNullException(nameof(identityConfiguration));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<string> GetClientCredentialsTokenAsync()
        {
            return await GetTokenAsync((client, disco) =>
            {
                var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = _identityConfiguration.ClientId,
                    ClientSecret = _identityConfiguration.ClientSecret,
                    Scope = _identityConfiguration.Scopes
                };

                return client.RequestClientCredentialsTokenAsync(tokenRequest);
            });
        }

        public async Task<string> GetTenantClientCredentialsTokenAsync(int tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentOutOfRangeException(nameof(tenantId));

            return await GetTokenAsync((client, disco) =>
            {
                var tokenRequest = new TokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _identityConfiguration.ClientId,
                    ClientSecret = _identityConfiguration.ClientSecret,
                };

                tokenRequest.Parameters.Add(OidcConstants.TokenRequest.GrantType, "tenant_client_credentials");
                tokenRequest.Parameters.Add(OidcConstants.TokenRequest.Scope, _identityConfiguration.Scopes);
                tokenRequest.Parameters.Add("tenant_id", tenantId.ToString(CultureInfo.InvariantCulture));

                return client.RequestTokenAsync(tokenRequest);
            });
        }

        private async Task<string> GetTokenAsync(Func<HttpClient, DiscoveryDocumentResponse, Task<TokenResponse>> tokenResponseProvider)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var discoveryDocumentRequest = new DiscoveryDocumentRequest
                {
                    Address = _identityConfiguration.AuthorityUri,
                    Policy =
                    {
                        ValidateIssuerName = false,
                        RequireHttps = false
                    }
                };

                var disco = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest);
                if (disco.IsError)
                    throw new CollaborationException(disco.Error);

                if (!disco.Issuer.Equals(_identityConfiguration.Issuer, StringComparison.OrdinalIgnoreCase))
                    throw new CollaborationException("Invalid issuer. Issuer=" + disco.Issuer);

                var response = await tokenResponseProvider(client, disco);

                if (response.IsError)
                    throw new CollaborationException(response.Error);

                return response.AccessToken;
            }
        }
    }
}
