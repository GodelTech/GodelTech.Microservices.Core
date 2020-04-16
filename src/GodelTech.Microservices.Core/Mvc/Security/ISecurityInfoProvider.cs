using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public interface ISecurityInfoProvider
    {
        IReadOnlyDictionary<string, AuthorizationPolicy> CreatePolicies();
        IReadOnlyDictionary<string, string> GetSupportedScopes();
    }
}