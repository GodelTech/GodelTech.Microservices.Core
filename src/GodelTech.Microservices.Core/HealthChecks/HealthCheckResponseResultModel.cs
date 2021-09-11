using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthCheck response result model.
    /// </summary>
    public class HealthCheckResponseResultModel
    {
        /// <summary>
        /// Status.
        /// </summary>
        public HealthStatus Status { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }
    }
}