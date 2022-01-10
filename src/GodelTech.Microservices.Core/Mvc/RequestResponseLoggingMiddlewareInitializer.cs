using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// RequestResponseLoggingMiddleware initializer.
    /// </summary>
    public class RequestResponseLoggingMiddlewareInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestResponseLoggingOptions>(ConfigureRequestResponseLoggingOptions);

            services.AddSingleton<RecyclableMemoryStreamManager>();
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        /// <summary>
        /// Configure RequestResponseLoggingOptions.
        /// </summary>
        /// <param name="options">RequestResponseLoggingOptions.</param>
        protected virtual void ConfigureRequestResponseLoggingOptions(RequestResponseLoggingOptions options)
        {

        }
    }
}