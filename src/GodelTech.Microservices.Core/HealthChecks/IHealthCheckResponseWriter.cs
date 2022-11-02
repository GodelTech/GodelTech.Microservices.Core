using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthCheck response writer.
    /// </summary>
    public interface IHealthCheckResponseWriter
    {
        /// <summary>
        /// Write response.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        /// <param name="healthReport">HealthReport.</param>
        Task WriteAsync(HttpContext context, HealthReport healthReport);
    }
}
