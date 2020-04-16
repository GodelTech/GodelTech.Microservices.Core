using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Http;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public sealed class SecurityContext : ISecurityContext
    {
        private static readonly AsyncLocal<ImmutableArray<Claim>> ImpersonatedClaims = new AsyncLocal<ImmutableArray<Claim>>();

        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        public int? TenantId => GetIntOrDefault(ClaimNames.TenantId);

        public bool HasPermissions(params string[] permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            return permissions.All(x => ClaimExists(ClaimNames.Permission, x));
        }

        public bool HasAnyPermission(params string[] permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            return permissions.Any(x => ClaimExists(ClaimNames.Permission, x));
        }

        public IDisposable Impersonate(params Claim[] claims)
        {
            if (claims == null) 
                throw new ArgumentNullException(nameof(claims));

            var currentClaimStorage = ImpersonatedClaims.Value;

            ImpersonatedClaims.Value = ImmutableArray.Create(claims);

            return new DisposableAction(() => { ImpersonatedClaims.Value = currentClaimStorage; });
        }

        private bool ClaimExists(string type, string value)
        {
            return GetClaims().Any(x =>
                x.Type.Equals(type, StringComparison.OrdinalIgnoreCase) &&
                x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        private int? GetIntOrDefault(string claimName)
        {
            var value = GetClaims().FirstOrDefault(x => x.Type.Equals(claimName, StringComparison.OrdinalIgnoreCase));
            if (value == null)
                return null;

            return int.Parse(value.Value);
        }

        private ImmutableArray<Claim> GetClaims()
        {
            var impersonatedClaims = ImpersonatedClaims.Value;

            if (impersonatedClaims == null)
                return ImmutableArray.CreateRange(GetClaimsFromHttpContext());

            return impersonatedClaims;
        }

        private IEnumerable<Claim> GetClaimsFromHttpContext()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
        }
    }
}
