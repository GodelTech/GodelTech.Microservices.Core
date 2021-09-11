using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthChecks initializer.
    /// </summary>
    public class HealthChecksInitializer : IMicroserviceInitializer
    {
        private readonly string _path;
        private readonly Action<HealthCheckOptions, IApplicationBuilder> _configure;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthChecksInitializer"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="configure">An <see cref="Action{HealthCheckOptions}"/> to configure the provided <see cref="HealthCheckOptions"/>.</param>
        public HealthChecksInitializer(
            string path = "/health",
            Action<HealthCheckOptions, IApplicationBuilder> configure = null)
        {
            _path = path;
            _configure = configure;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHealthCheckResponseWriter, HealthCheckResponseWriter>();

            services.AddHealthChecks();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">WebHost environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            var options = new HealthCheckOptions();

            ConfigureHealthCheckOptions(options, app);

            _configure?.Invoke(options, app);

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapHealthChecks(
                        _path,
                        options
                    );
                }
            );
        }

        /// <summary>
        /// Configure HealthCheckOptions.
        /// </summary>
        /// <param name="options">HealthCheckOptions.</param>
        /// <param name="app">ApplicationBuilder.</param>
        protected virtual void ConfigureHealthCheckOptions(HealthCheckOptions options, IApplicationBuilder app)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (app == null) throw new ArgumentNullException(nameof(app));

            var writer = app.ApplicationServices.GetRequiredService<IHealthCheckResponseWriter>();

            options.AllowCachingResponses = false;
            options.Predicate = _ => true;
            options.ResponseWriter = writer.Write;
        }
    }
}