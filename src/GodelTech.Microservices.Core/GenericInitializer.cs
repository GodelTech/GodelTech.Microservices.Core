using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Generic Microservice initializer.
    /// </summary>
    public sealed class GenericInitializer : IMicroserviceInitializer
    {
        private readonly Action<IServiceCollection> _configureServices;
        private readonly Action<IApplicationBuilder, IWebHostEnvironment> _configure;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInitializer"/> class.
        /// </summary>
        /// <param name="configureServices">Action for configure services.</param>
        /// <param name="configure">Action for configure.</param>
        public GenericInitializer(
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder, IWebHostEnvironment> configure = null)
        {
            _configureServices = configureServices;
            _configure = configure;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInitializer"/> class.
        /// </summary>
        /// <param name="configureServices"></param>
        public GenericInitializer(
            Action<IServiceCollection> configureServices)
            : this(configureServices, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInitializer"/> class.
        /// </summary>
        /// <param name="configure">Action for configure.</param>
        public GenericInitializer(
            Action<IApplicationBuilder, IWebHostEnvironment> configure)
            : this(null, configure)
        {

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            _configureServices?.Invoke(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">WebHost environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _configure?.Invoke(app, env);
        }
    }
}