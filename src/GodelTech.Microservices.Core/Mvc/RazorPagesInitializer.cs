using System;
using Microsoft.AspNetCore.Builder;
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
        private readonly Action<RazorPagesOptions> _configureRazorPages;
        private readonly Action<IMvcBuilder> _configureBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorPagesInitializer"/> class.
        /// </summary>
        /// <param name="configureRazorPages">An <see cref="Action{RazorPagesOptions}"/> to configure the provided <see cref="RazorPagesOptions"/>.</param>
        /// <param name="configureBuilder">An <see cref="Action{IMvcBuilder}"/> to configure the provided <see cref="IMvcBuilder"/>.</param>
        public RazorPagesInitializer(
            Action<RazorPagesOptions> configureRazorPages = null,
            Action<IMvcBuilder> configureBuilder = null)
        {
            _configureRazorPages = configureRazorPages;
            _configureBuilder = configureBuilder;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services
                .AddRazorPages(
                    options =>
                    {
                        ConfigureRazorPagesOptions(options);

                        _configureRazorPages?.Invoke(options);
                    }
                );

            _configureBuilder?.Invoke(builder);
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

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
    }
}