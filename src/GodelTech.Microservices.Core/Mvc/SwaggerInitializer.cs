using System;
using System.Linq;
using GodelTech.Microservices.Core.Mvc.Security;
using GodelTech.Microservices.Core.Mvc.Swagger;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GodelTech.Microservices.Core.Mvc
{
    public class SwaggerInitializer : MicroserviceInitializerBase
    {
        private readonly ISecurityInfoProvider _securityInfoProvider;

        public SwaggerInitializer(IConfiguration configuration, ISecurityInfoProvider securityInfoProvider)
            : base(configuration)
        {
            _securityInfoProvider = securityInfoProvider ?? throw new ArgumentNullException(nameof(securityInfoProvider));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Details can be found here https://github.com/domaindrivendev/Swashbuckle.AspNetCore
            // Default address http://localhost:5000/swagger/
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var identityConfig = Configuration.GetIdentityConfiguration();
            var scopes = _securityInfoProvider.GetSupportedScopes().ToDictionary(x => x.Key, x => x.Value);

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(OAuth2Security.OAuth2, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\""
                });

                options.AddSecurityDefinition(OAuth2Security.AuthorizationCode, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(identityConfig.GetPublicAuthorizeEndpoint()),
                            TokenUrl = new Uri(identityConfig.GetPublicTokenEndpoint()),
                            Scopes = scopes
                        }
                    }
                });

                options.AddSecurityDefinition(OAuth2Security.Implicit, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(identityConfig.GetPublicAuthorizeEndpoint()),
                            Scopes = scopes
                        }
                    }
                });

                options.AddSecurityDefinition(OAuth2Security.ResourceOwnerPasswordCredentials, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(identityConfig.GetPublicAuthorizeEndpoint()),
                            TokenUrl = new Uri(identityConfig.GetPublicTokenEndpoint()),
                            Scopes = scopes
                        },
                    }
                });

                options.AddSecurityDefinition(OAuth2Security.ClientCredentials, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(identityConfig.GetPublicTokenEndpoint()),
                            Scopes = scopes
                        }
                    }
                });

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReviewItEasy API", Version = "v1" });

                options.EnableAnnotations();

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }
    }
}
