using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using GodelTech.Microservices.Core.Mvc.Security;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GodelTech.Microservices.Core.Mvc
{
    public class ApiSecurityInitializer : MicroserviceInitializerBase
    {
        private readonly ISecurityInfoProvider _securityInfoProvider;

        protected virtual SecurityProtocolType SecurityProtocol => SecurityProtocolType.Tls12;
        protected virtual IReadOnlyDictionary<string, string[]> ServicePolicies => new Dictionary<string, string[]>();

        public ApiSecurityInitializer(IConfiguration configuration, ISecurityInfoProvider securityInfoProvider)
            : base(configuration)
        {
            _securityInfoProvider = securityInfoProvider ?? throw new ArgumentNullException(nameof(securityInfoProvider));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var config = Configuration.GetIdentityConfiguration();

            services.AddSingleton(config);

            ServicePointManager.SecurityProtocol = SecurityProtocol;

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = config.AuthorityUri;
                    options.Audience = config.Audience;
                    options.IncludeErrorDetails = true;
                    options.RequireHttpsMetadata = config.RequireHttpsMetadata;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = config.Issuer,
                            ValidAudience = options.Audience,
                        };

                    options.SaveToken = true;
                });

            services.AddAuthorization(ConfigureAuthorization);
        }

        protected virtual void ConfigureAuthorization(AuthorizationOptions options)
        {
            foreach (var (policyName, policy) in _securityInfoProvider.CreatePolicies())
            {
                options.AddPolicy(policyName, policy);
            }
        }
    }
}
