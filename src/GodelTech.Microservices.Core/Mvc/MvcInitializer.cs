using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    public class MvcInitializer : MicroserviceInitializerBase
    {
        public bool EnableAddRazorRuntimeCompilation { get; set; }

        public MvcInitializer(IConfiguration configuration) 
            : base(configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));

            var builder = services.AddControllersWithViews(ConfigureMvcOptions);

            if (EnableAddRazorRuntimeCompilation)
                builder.AddRazorRuntimeCompilation();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (env == null)
                throw new ArgumentNullException(nameof(env));

            app.UseEndpoints(ConfigureEndpoints);
        }

        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {

        }

        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
