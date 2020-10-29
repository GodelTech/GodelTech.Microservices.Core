using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    public class RazorPagesInitializer : MicroserviceInitializerBase
    {
        public bool EnableAddRazorRuntimeCompilation { get; set; }

        public RazorPagesInitializer(IConfiguration configuration) 
            : base(configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var builder = services.AddRazorPages(ConfigureRazorPagesOptions);

            if (EnableAddRazorRuntimeCompilation)
                builder.AddRazorRuntimeCompilation();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapRazorPages();
                }
            );
        }

        /// <summary>
        /// Configure Razor pages options.
        /// You can put security configuration logic here in your subclass
        /// options.Conventions.AuthorizePage("/Index");
        /// options.Conventions.AuthorizeFolder("/Admin");
        /// </summary>
        /// <param name="options">Razor pages options</param>
        protected virtual void ConfigureRazorPagesOptions(RazorPagesOptions options)
        {

        }
    }
}
