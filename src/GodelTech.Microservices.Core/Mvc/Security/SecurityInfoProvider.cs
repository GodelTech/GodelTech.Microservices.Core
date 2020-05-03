using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public class SecurityInfoProvider : ISecurityInfoProvider
    {
        private readonly IReadOnlyDictionary<string, string[]> _policies;

        public SecurityInfoProvider(IReadOnlyDictionary<string, string[]> policies)
        {
            _policies = policies ?? throw new ArgumentNullException(nameof(policies));
        }

        public IReadOnlyDictionary<string, AuthorizationPolicy> CreatePolicies()
        {
            return _policies.ToDictionary(x => x.Key, x => CreatePolicy(x.Value));
        }

        protected AuthorizationPolicy CreatePolicy(params string[] scopes)
        {
            var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();

            authorizationPolicyBuilder.RequireAuthenticatedUser();
            authorizationPolicyBuilder.RequireClaim("scope", scopes);

            var permissions = GetPermissionsForScopes(scopes);

            if (permissions.Any())
                authorizationPolicyBuilder.AddRequirements(new PermissionRequirement(permissions));

            return authorizationPolicyBuilder.Build();
        }

        private static string[] GetPermissionsForScopes(string[] scopes)
        {
            return
                (from scope in scopes
                let permission = Scopes.GetPermissionByScope(scope)
                where !string.IsNullOrWhiteSpace(permission)
                select permission)
                .ToArray();
        }
    }
}