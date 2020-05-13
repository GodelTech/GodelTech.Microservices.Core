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

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    Predicate = _ => true,
                    ResponseWriter = WriteResponse
                });
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
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

            var json = JsonSerializer.Serialize(healthResult, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return httpContext.Response.WriteAsync(json);
        }
    }
}
