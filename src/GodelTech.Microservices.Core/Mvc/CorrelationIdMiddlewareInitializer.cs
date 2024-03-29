﻿using GodelTech.Microservices.Core.Mvc.CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// CorrelationIdMiddleware initializer.
    /// </summary>
    public class CorrelationIdMiddlewareInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CorrelationIdOptions>(ConfigureCorrelationIdOptions);

            services.AddSingleton<ICorrelationIdContextAccessor, CorrelationIdContextAccessor>();
            services.AddSingleton<ICorrelationIdContextFactory, CorrelationIdContextFactory>();
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        /// <summary>
        /// Configure CorrelationIdOptions.
        /// </summary>
        /// <param name="options">CorrelationIdOptions.</param>
        protected virtual void ConfigureCorrelationIdOptions(CorrelationIdOptions options)
        {

        }
    }
}
