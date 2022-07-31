using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Abstract microservice startup.
    /// </summary>
    public abstract class MicroserviceStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceStartup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        protected MicroserviceStartup(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Configuration = configuration;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            foreach (var initializer in CreateInitializers())
            {
                initializer.ConfigureServices(services);
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">WebHost environment</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var initializer in CreateInitializers())
            {
                initializer.Configure(app, env);
            }

            foreach (var initializer in CreateInitializers())
            {
                initializer.ConfigureEndpoints(app, env);
            }
        }

        /// <summary>
        /// Creates microservice initializers.
        /// </summary>
        /// <returns>List of microservice initializers.</returns>
        protected abstract IEnumerable<IMicroserviceInitializer> CreateInitializers();
    }
}
