using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GodelTech.Microservices.Core.Mvc.Swagger
{
    public class AuthorizeCheckOperationFilter : AttributeAwareOperationFilter, IOperationFilter, IAuthorizeCheckOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            var scopeAttributes = GetMethodOrControllerAttributes<SwaggerRequiredScopesAttribute>(apiDescription).ToArray();

            // skip if operation does not have ScopeAttribute
            if (!scopeAttributes.Any())
                return;

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            var scopes =
                (from item in scopeAttributes
                 from scope in item.Scopes
                 select scope)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = OAuth2Security.OAuth2,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        scopes
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = OAuth2Security.AuthorizationCode,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        scopes
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = OAuth2Security.Implicit,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        scopes
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = OAuth2Security.ClientCredentials,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        scopes
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = OAuth2Security.ResourceOwnerPasswordCredentials,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        scopes
                    }
                }
            };
        }
    }
}