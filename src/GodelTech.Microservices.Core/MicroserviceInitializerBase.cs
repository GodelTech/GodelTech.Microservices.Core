using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Abstract microservice initializer.
    /// </summary>
    public abstract class MicroserviceInitializerBase : IMicroserviceInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceInitializerBase"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        protected MicroserviceInitializerBase(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Configuration = configuration;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
