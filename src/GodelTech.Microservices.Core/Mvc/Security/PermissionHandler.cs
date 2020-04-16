using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!context.User?.Identity?.IsAuthenticated ?? false)
            {
                context.Fail();
                return Task.CompletedTask;
            }    

            if (requirement.Permissions.All(x => HasPermission(context.User.Claims, x)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private static bool HasPermission(IEnumerable<Claim> claims, string permission)
        {
            return claims.Any(x =>
                x.Type.Equals(ClaimNames.Permission, StringComparison.OrdinalIgnoreCase) &&
                x.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }
    }
}