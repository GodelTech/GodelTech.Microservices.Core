using System;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc
{
    public class FakeApiInitializer : ApiInitializer
    {
        private readonly Action _onConfigureResponseCachingOptions;
        private readonly Action _onConfigureMemoryCacheOptions;

        public FakeApiInitializer(
            Action onConfigureResponseCachingOptions = null,
            Action onConfigureMemoryCacheOptions = null)
        {
            _onConfigureResponseCachingOptions = onConfigureResponseCachingOptions;
            _onConfigureMemoryCacheOptions = onConfigureMemoryCacheOptions;
        }

        protected override void ConfigureResponseCachingOptions(ResponseCachingOptions options)
        {
            base.ConfigureResponseCachingOptions(options);

            _onConfigureResponseCachingOptions?.Invoke();
        }

        protected override void ConfigureMemoryCacheOptions(MemoryCacheOptions options)
        {
            base.ConfigureMemoryCacheOptions(options);

            _onConfigureMemoryCacheOptions?.Invoke();
        }
    }
}
