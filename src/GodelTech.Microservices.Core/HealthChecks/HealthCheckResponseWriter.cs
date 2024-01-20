using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private static readonly JsonSerializerOptions WriteJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        /// <inheritdoc />
        public Task WriteAsync(HttpContext context, HealthReport healthReport)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(healthReport);

            context.Response.ContentType = "application/json";

            var healthResult = new HealthCheckResponseModel
            {
                Status = healthReport.Status,
                Results = healthReport.Entries
                    .Select(
                        x => new KeyValuePair<string, HealthCheckResponseResultModel>(
                            x.Key,
                            new HealthCheckResponseResultModel
                            {
                                Status = x.Value.Status,
                                Description = x.Value.Description
                            }
                        )
                    ).ToList(),
                TotalDuration = healthReport.TotalDuration.TotalMilliseconds
            };

            var json = JsonSerializer.Serialize(
                healthResult,
                WriteJsonSerializerOptions
            );

            return context.Response.WriteAsync(json);
        }
    }
}
