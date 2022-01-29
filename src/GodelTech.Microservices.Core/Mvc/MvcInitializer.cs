﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// Mvc initializer.
    /// </summary>
    public class MvcInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var builder = services
                .AddControllersWithViews(
                    options =>
                    {
                        ConfigureMvcOptions(options);
                    }
                );

            ConfigureMvcBuilder(builder);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(
                endpoints =>
                {
                    ConfigureEndpoints(endpoints);
                }
            );
        }

        /// <summary>
        /// Configure MvcOptions.
        /// </summary>
        /// <param name="options">MvcOptions.</param>
        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.SuppressAsyncSuffixInActionNames = false;
        }

        /// <summary>
        /// Configure MvcBuilder.
        /// </summary>
        /// <param name="builder">IMvcBuilder.</param>
        protected virtual void ConfigureMvcBuilder(IMvcBuilder builder)
        {

        }

        /// <summary>
        /// Configure Endpoints
        /// </summary>
        /// <param name="endpoints">IEndpointRouteBuilder.</param>
        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}