using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Abstract microservice initializers.
    /// </summary>
    public abstract class MicroserviceInitializerCollectionBase : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            foreach (var initializer in CreateInitializers())
            {
                initializer.ConfigureServices(services);
            }
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var initializer in CreateInitializers())
            {
                initializer.Configure(app, env);
            }

            ConfigureEndpoints(app, env);
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
