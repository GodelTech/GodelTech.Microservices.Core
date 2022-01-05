using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeRequestResponseLoggingMiddlewareInitializer : RequestResponseLoggingMiddlewareInitializer
    {
        public void ExposedConfigureRequestResponseLoggingOptions(RequestResponseLoggingOptions options)
        {
            base.ConfigureRequestResponseLoggingOptions(options);
        }
    }
}