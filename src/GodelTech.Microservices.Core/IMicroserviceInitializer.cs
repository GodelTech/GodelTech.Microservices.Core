using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("GodelTech.Microservices.Core.Tests")]
namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Microservice initializer.
    /// </summary>
    public interface IMicroserviceInitializer
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">WebHost environment.</param>
        void Configure(IApplicationBuilder app, IWebHostEnvironment env);

        /// <summary>
        /// Use this method to configure endpoints.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">WebHost environment.</param>
        void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // to be implemented in derived type
        }
    }
}
