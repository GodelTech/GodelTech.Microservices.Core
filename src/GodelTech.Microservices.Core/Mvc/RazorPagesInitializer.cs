using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
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
            services.AddResponseCaching(
                options =>
                {
                    ConfigureResponseCachingOptions(options);
                }
            );

            services.AddMemoryCache(
                options =>
                {
                    ConfigureMemoryCacheOptions(options);
                }
            );

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
        /// Configure ResponseCachingOptions.
        /// </summary>
        /// <param name="options">ResponseCachingOptions.</param>
        protected virtual void ConfigureResponseCachingOptions(ResponseCachingOptions options)
        {

        }

        /// <summary>
        /// Configure MemoryCacheOptions.
        /// </summary>
        /// <param name="options">MemoryCacheOptions.</param>
        protected virtual void ConfigureMemoryCacheOptions(MemoryCacheOptions options)
        {

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
