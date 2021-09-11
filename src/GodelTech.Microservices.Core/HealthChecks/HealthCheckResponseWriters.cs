using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthCheck response writer.
    /// </summary>
    public class HealthCheckResponseWriter : IHealthCheckResponseWriter
    {
        /// <inheritdoc />
        public Task Write(HttpContext context, HealthReport healthReport)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (healthReport == null) throw new ArgumentNullException(nameof(healthReport));

            context.Response.ContentType = "application/json";

            // todo: a.solonoy: why not just return HealthReport model?
            var healthResult = new
            {
                Status = healthReport.Status.ToString(),
                Results = healthReport.Entries.ToDictionary(
                    x => x.Key,
                    x => new
                    {
                        Status = x.Value.Status.ToString(),
                        x.Value.Description
                    }
                ).ToArray(),
                TotalDuration = healthReport.TotalDuration.TotalMilliseconds
            };

            var json = JsonSerializer.Serialize(
                healthResult,
                new JsonSerializerOptions
                {
                    // todo: a.solonoy: do we need this?
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );

            return context.Response.WriteAsync(json);
        }
    }
}