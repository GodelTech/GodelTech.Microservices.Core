using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.HealthChecks
{
    public class RoutingInitializer : MicroserviceInitializerBase
    {
        public RoutingInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
        }
    }
}