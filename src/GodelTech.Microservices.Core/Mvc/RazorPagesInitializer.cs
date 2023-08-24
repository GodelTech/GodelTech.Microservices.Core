﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// Razor Pages initializer.
    /// </summary>
    public class RazorPagesInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Stryker disable once statement
            services.AddResponseCaching();

            // Stryker disable once statement
            services.AddMemoryCache();

            var builder = services
                .AddRazorPages(
                    // Stryker disable once block
                    options =>
                    {
                        // Stryker disable once statement
                        ConfigureRazorPagesOptions(options);
                    }
                );

            ConfigureMvcBuilder(builder);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapRazorPages();
                }
            );
        }

        /// <summary>
        /// Configure RazorPagesOptions.
        /// </summary>
        /// <param name="options">RazorPagesOptions.</param>
        protected virtual void ConfigureRazorPagesOptions(RazorPagesOptions options)
        {

        }

        /// <summary>
        /// Configure MvcBuilder.
        /// </summary>
        /// <param name="builder">IMvcBuilder.</param>
        protected virtual void ConfigureMvcBuilder(IMvcBuilder builder)
        {

        }
    }
}
