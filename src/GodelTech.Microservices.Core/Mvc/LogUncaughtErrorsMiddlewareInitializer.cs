using GodelTech.Microservices.Core.Mvc.LogUncaughtErrors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// LogUncaughtErrorsMiddleware initializer.
    /// </summary>
    public class LogUncaughtErrorsMiddlewareInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<LogUncaughtErrorsMiddleware>();
        }
    }
}
