using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    public class GenericInitializer : IMicroserviceInitializer
    {
        private readonly Action<IServiceCollection> _configureServices;
        private readonly Action<IApplicationBuilder, IWebHostEnvironment> _configureAction;

        public GenericInitializer(
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder, IWebHostEnvironment> configureAction = null)
        {
            _configureServices = configureServices;
            _configureAction = configureAction;
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
