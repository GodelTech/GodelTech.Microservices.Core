using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.CorrelationId;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeCorrelationIdMiddlewareInitializer : CorrelationIdMiddlewareInitializer
    {
        public void ExposedConfigureCorrelationIdOptions(CorrelationIdOptions options)
        {
            base.ConfigureCorrelationIdOptions(options);
        }
    }
}