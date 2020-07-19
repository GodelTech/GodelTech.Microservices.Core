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

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddRazorPages(ConfigureRazorPagesOptions);

            if (EnableAddRazorRuntimeCompilation)
                builder.AddRazorRuntimeCompilation();
        }

        protected virtual void ConfigureRazorPagesOptions(RazorPagesOptions options)
        {
            // You can put security configuration logic here in your subclass
            // options.Conventions.AuthorizePage("/Index");
            // options.Conventions.AuthorizeFolder("/Admin");
        }
    }
}