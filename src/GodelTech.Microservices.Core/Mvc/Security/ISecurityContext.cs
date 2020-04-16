using System;
using System.Security.Claims;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public interface ISecurityContext
    {
        int? TenantId { get; }
        bool HasPermissions(params string[] permissions);
        bool HasAnyPermission(params string[] permissions);

        IDisposable Impersonate(params Claim[] claims);
    }
}