﻿using Microsoft.AspNetCore.Builder;
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

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(ConfigureEndpoints);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews(ConfigureMvcOptions);

            if (EnableAddRazorRuntimeCompilation)
                builder.AddRazorRuntimeCompilation();
        }

        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
        }

        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}