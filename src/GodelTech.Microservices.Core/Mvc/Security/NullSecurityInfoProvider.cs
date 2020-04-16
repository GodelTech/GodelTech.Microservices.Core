using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public class NullSecurityInfoProvider : ISecurityInfoProvider
    {
        public IReadOnlyDictionary<string, AuthorizationPolicy> CreatePolicies()
        {
            return new Dictionary<string, AuthorizationPolicy>();
        }

        public IReadOnlyDictionary<string, string> GetSupportedScopes()
        {
            return new Dictionary<string, string>();
        }
    }
}