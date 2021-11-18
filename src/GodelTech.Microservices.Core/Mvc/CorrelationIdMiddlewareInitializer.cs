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
            services.AddSingleton<ICorrelationIdContextAccessor, CorrelationIdContextAccessor>();
            services.AddSingleton<ICorrelationIdContextFactory, CorrelationIdContextFactory>();
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}