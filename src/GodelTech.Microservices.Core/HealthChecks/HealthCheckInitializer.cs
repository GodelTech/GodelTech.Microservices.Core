using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.HealthChecks
{
    /// <summary>
    /// HealthCheck initializer.
    /// </summary>
    public class HealthCheckInitializer : IMicroserviceInitializer
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckInitializer"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        public HealthCheckInitializer(string path = "/health")
        {
            _path = path;
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHealthCheckResponseWriter, HealthCheckResponseWriter>();

            services.AddHealthChecks();
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new HealthCheckOptions();

            ConfigureHealthCheckOptions(options, app);

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
            options.ResponseWriter = writer.WriteAsync;
        }
    }
}
