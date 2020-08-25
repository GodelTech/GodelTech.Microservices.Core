using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    public abstract class MicroserviceStartup
    {
        private IMicroserviceInitializer[] _initializers;

        private IEnumerable<IMicroserviceInitializer> Initializers => _initializers ?? (_initializers = CreateInitializers().ToArray());

        public IConfiguration Configuration { get; }

        protected MicroserviceStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected abstract IEnumerable<IMicroserviceInitializer> CreateInitializers();
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var initializer in Initializers)
            {
                initializer.Configure(app, env);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            foreach (var initializer in Initializers)
            {
                initializer.ConfigureServices(services);
            }
        }
    }
}
