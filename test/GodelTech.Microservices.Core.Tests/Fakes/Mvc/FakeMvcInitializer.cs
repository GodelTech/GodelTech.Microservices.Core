using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeMvcInitializer : MvcInitializer
    {
        public void ExposedConfigureMvcOptions(MvcOptions options)
        {
            base.ConfigureMvcOptions(options);
        }

        public void ExposedConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(endpoints);
        }
    }
}