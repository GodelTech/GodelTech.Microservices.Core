using GodelTech.Microservices.Core.Mvc.CorrelationId;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CorrelationIdOptions>(ConfigureCorrelationIdOptions);

            services.AddSingleton<ICorrelationIdContextAccessor, CorrelationIdContextAccessor>();
            services.AddSingleton<ICorrelationIdContextFactory, CorrelationIdContextFactory>();
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
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