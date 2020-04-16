using System;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Services
{
    public static class IdentityConfigurationExtensions
    {
        public static IIdentityConfiguration GetIdentityConfiguration(this IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var identityConfig = new IdentityConfiguration();

            configuration.Bind("IdentityConfiguration", identityConfig);

            return identityConfig;
        }

        public static string GetPublicTokenEndpoint(this IIdentityConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return ReplaceDomainAndPort(configuration) + "connect/token";
        }

        public static string GetPublicAuthorizeEndpoint(this IIdentityConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return ReplaceDomainAndPort(configuration) + "connect/authorize";
        }

        private static string ReplaceDomainAndPort(IIdentityConfiguration configuration)
        {
            var publicAuthorityAddress = configuration.PublicAuthorityUri;

            if (string.IsNullOrWhiteSpace(publicAuthorityAddress))
                return configuration.AuthorityUri.EndsWith("/")
                    ? configuration.AuthorityUri
                    : configuration.AuthorityUri + "/";

            publicAuthorityAddress = publicAuthorityAddress.EndsWith("/") ?
                publicAuthorityAddress.Substring(publicAuthorityAddress.Length - 1) :
                publicAuthorityAddress;

            return publicAuthorityAddress + new Uri(configuration.AuthorityUri).PathAndQuery;
        }
    }
}