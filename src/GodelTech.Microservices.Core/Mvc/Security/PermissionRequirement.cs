using System;
using Microsoft.AspNetCore.Authorization;

namespace GodelTech.Microservices.Core.Mvc.Security
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string[] Permissions { get; }

        public PermissionRequirement(params string[] permissions)
        {
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }
    }
}