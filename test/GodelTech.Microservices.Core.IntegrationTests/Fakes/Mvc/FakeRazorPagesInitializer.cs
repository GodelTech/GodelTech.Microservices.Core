﻿using System;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc
{
    public class FakeRazorPagesInitializer : RazorPagesInitializer
    {
        private readonly Action _onConfigureResponseCachingOptions;
        private readonly Action _onConfigureMemoryCacheOptions;
        private readonly Action _onConfigureRazorPagesOptions;

        public FakeRazorPagesInitializer(
            Action onConfigureResponseCachingOptions = null,
            Action onConfigureMemoryCacheOptions = null,
            Action onConfigureRazorPagesOptions = null)
        {
            _onConfigureResponseCachingOptions = onConfigureResponseCachingOptions;
            _onConfigureMemoryCacheOptions = onConfigureMemoryCacheOptions;
            _onConfigureRazorPagesOptions = onConfigureRazorPagesOptions;
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

        protected override void ConfigureRazorPagesOptions(RazorPagesOptions options)
        {
            base.ConfigureRazorPagesOptions(options);

            _onConfigureRazorPagesOptions?.Invoke();
        }

        protected override void ConfigureMvcBuilder(IMvcBuilder builder)
        {
            builder
                .AddApplicationPart(typeof(TestStartup).Assembly)
                .AddRazorRuntimeCompilation();
        }
    }
}
