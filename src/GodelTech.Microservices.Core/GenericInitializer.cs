using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    public class GenericInitializer : IMicroserviceInitializer
    {
        private readonly Action<IApplicationBuilder, IWebHostEnvironment> _configureAction;
        private readonly Action<IServiceCollection> _configureServices;

        public GenericInitializer(
            Action<IApplicationBuilder, IWebHostEnvironment> configureAction = null,
            Action<IServiceCollection> configureServices = null)
        {
            _configureAction = configureAction;
            _configureServices = configureServices;
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _configureAction?.Invoke(app, env);

        }

        public void ConfigureServices(IServiceCollection services)
        {
            _configureServices?.Invoke(services);
        }
    }
}