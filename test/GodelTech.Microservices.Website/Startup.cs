using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Website
{
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);

            // Uncomment this line if HTTPs usage is required
            // yield return new HttpsInitializer(Configuration);

            yield return new GenericInitializer((app, env) => app.UseStaticFiles());
            yield return new GenericInitializer((app, env) => app.UseRouting());
            yield return new GenericInitializer((app, env) => app.UseAuthentication());

            yield return new ApiInitializer(Configuration);
            //yield return new RazorPagesInitializer(Configuration);
            yield return new MvcInitializer(Configuration)
            {
                EnableAddRazorRuntimeCompilation = true
            };
        }
    }
}
