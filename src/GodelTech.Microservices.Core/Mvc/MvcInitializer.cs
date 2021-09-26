using System;
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
        private readonly Action<MvcOptions> _configureMvc;
        private readonly Action<IMvcBuilder> _configureBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiInitializer"/> class.
        /// </summary>
        /// <param name="configureMvc">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <param name="configureBuilder">An <see cref="Action{IMvcBuilder}"/> to configure the provided <see cref="IMvcBuilder"/>.</param>
        public MvcInitializer(
            Action<MvcOptions> configureMvc = null,
            Action<IMvcBuilder> configureBuilder = null)
        {
            _configureMvc = configureMvc;
            _configureBuilder = configureBuilder;
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var builder = services
                .AddControllersWithViews(
                    options =>
                    {
                        ConfigureMvcOptions(options);

                        _configureMvc?.Invoke(options);
                    }
                );

            _configureBuilder?.Invoke(builder);
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

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

            // todo: a.solonoy: do we need this? By default it is true.
            options.SuppressAsyncSuffixInActionNames = false;
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