using System;
using System.Collections.Generic;
using System.Linq;

namespace GodelTech.Microservices.Core.Services
{
    public class IdentityConfiguration : IIdentityConfiguration
    {
        public string AuthorityUri { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string Issuer { get; set; }
        public string PublicAuthorityUri { get; set; }

        public IEnumerable<string> ScopesAsList =>
            (Scopes ?? String.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());
    }
}
