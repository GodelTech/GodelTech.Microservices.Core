using System;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc
{
    public class FakeRequestResponseLoggingMiddlewareInitializer : RequestResponseLoggingMiddlewareInitializer
    {
        private readonly Action _onConfigureRequestResponseLoggingOptions;

        public FakeRequestResponseLoggingMiddlewareInitializer(Action onConfigureRequestResponseLoggingOptions = null)
        {
            _onConfigureRequestResponseLoggingOptions = onConfigureRequestResponseLoggingOptions;
        }

        protected override void ConfigureRequestResponseLoggingOptions(RequestResponseLoggingOptions options)
        {
            base.ConfigureRequestResponseLoggingOptions(options);

            _onConfigureRequestResponseLoggingOptions?.Invoke();
        }
    }
}
