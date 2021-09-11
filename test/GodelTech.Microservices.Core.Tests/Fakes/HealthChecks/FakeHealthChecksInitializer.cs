using GodelTech.Microservices.Core.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace GodelTech.Microservices.Core.Tests.Fakes.HealthChecks
{
    public class FakeHealthChecksInitializer : HealthChecksInitializer
    {
        public void ExposedConfigureHealthCheckOptions(HealthCheckOptions options, IApplicationBuilder app)
        {
            base.ConfigureHealthCheckOptions(options, app);
        }
    }
}