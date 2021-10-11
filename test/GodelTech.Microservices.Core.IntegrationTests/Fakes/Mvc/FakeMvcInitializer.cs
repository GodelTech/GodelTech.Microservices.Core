using GodelTech.Microservices.Core.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc
{
    public class FakeMvcInitializer : MvcInitializer
    {
        protected override void ConfigureMvcBuilder(IMvcBuilder builder)
        {
            builder
                .AddApplicationPart(typeof(TestStartup).Assembly)
                .AddRazorRuntimeCompilation();
        }
    }
}