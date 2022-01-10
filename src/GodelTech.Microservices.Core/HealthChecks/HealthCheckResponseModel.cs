using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthCheck response model.
    /// </summary>
    public class HealthCheckResponseModel
    {
        /// <summary>
        /// Status.
        /// </summary>
        public HealthStatus Status { get; set; }

        /// <summary>
        /// Results.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
                               // You can suppress the warning if the property is part of a Data Transfer Object (DTO) class.
        public IList<KeyValuePair<string, HealthCheckResponseResultModel>> Results { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Total duration in milliseconds.
        /// </summary>
        public double TotalDuration { get; set; }
    }
}