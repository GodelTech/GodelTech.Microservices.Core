using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeApiInitializer : ApiInitializer
    {
        public void ExposedConfigureMvcOptions(MvcOptions options)
        {
            base.ConfigureMvcOptions(options);
        }

        public void ExposedConfigureJsonOptions(JsonOptions options)
        {
            base.ConfigureJsonOptions(options);
        }
    }
}
