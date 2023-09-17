using System;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.CorrelationId;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc
{
    public class FakeCorrelationIdMiddlewareInitializer : CorrelationIdMiddlewareInitializer
    {
        private readonly Action _onConfigureCorrelationIdOptions;

        public FakeCorrelationIdMiddlewareInitializer(Action onConfigureCorrelationIdOptions = null)
        {
            _onConfigureCorrelationIdOptions = onConfigureCorrelationIdOptions;
        }

        protected override void ConfigureCorrelationIdOptions(CorrelationIdOptions options)
        {
            base.ConfigureCorrelationIdOptions(options);

            _onConfigureCorrelationIdOptions?.Invoke();
        }
    }
}
