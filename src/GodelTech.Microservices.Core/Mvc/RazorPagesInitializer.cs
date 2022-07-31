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
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            services.AddMemoryCache();

            var builder = services
                .AddRazorPages(
                    options =>
                    {
                        ConfigureRazorPagesOptions(options);
                    }
                );

            ConfigureMvcBuilder(builder);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();

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
