using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// DeveloperExceptionPage initializer.
    /// </summary>
    public class DeveloperExceptionPageInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}