using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeMvcInitializer : MvcInitializer
    {
        public void ExposedConfigureMvcOptions(MvcOptions options)
        {
            base.ConfigureMvcOptions(options);
        }

        public virtual void ExposedConfigureMvcBuilder(IMvcBuilder builder)
        {
            base.ConfigureMvcBuilder(builder);
        }

        public void ExposedConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(endpoints);
        }
    }
}
