using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.HealthChecks
{
    public class HealthCheckInitializer : MicroserviceInitializerBase
    {
        public HealthCheckInitializer(IConfiguration configuration)
            : base(configuration)
        {

        }

        public string HealthCheckPath { get; set; } = "/health";

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddHealthChecks();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapHealthChecks(
                        HealthCheckPath,
                        new HealthCheckOptions
                        {
                            AllowCachingResponses = false,
                            Predicate = _ => true,
                            ResponseWriter = WriteResponse
                        }
                    );
                }
            );
        }

        protected virtual Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            if (httpContext == null) 
                throw new ArgumentNullException(nameof(httpContext));
            if (result == null) 
                throw new ArgumentNullException(nameof(result));

            httpContext.Response.ContentType = "application/json";

            var healthResult = new
            {
                status = result.Status.ToString(),
                results = result.Entries.ToDictionary(
                    x => x.Key,
                    x => new
                    {
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description,
                    }
                ).ToArray()
            };

            var json = JsonSerializer.Serialize(
                healthResult,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );

            return httpContext.Response.WriteAsync(json);
        }
    }
}
