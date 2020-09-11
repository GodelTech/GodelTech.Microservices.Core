//using System.Collections.Generic;
//using GodelTech.Microservices.Core;
//using GodelTech.Microservices.Core.Mvc;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.Configuration;

//namespace GodelTech.Microservices.AwsLambda
//{
//    public class Startup2 : MicroserviceStartup
//    {
//        public Startup2(IConfiguration configuration)
//            : base(configuration)
//        {
//        }

//        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
//        {
//            yield return new DeveloperExceptionPageInitializer(Configuration);

//            // Uncomment this line if HTTPs usage is required
//            // yield return new HttpsInitializer(Configuration);

//            yield return new GenericInitializer((app, env) => app.UseStaticFiles());
//            yield return new GenericInitializer((app, env) => app.UseRouting());
//            yield return new GenericInitializer((app, env) => app.UseAuthentication());

//            yield return new ApiInitializer(Configuration);

//            // IMPORTANT: You can use Razor and MVC initializers together but you need to make sure
//            // that their routes do not conflict with each other. If the same url is mapped to MVC Controller
//            // and Page first initializer wins
//            yield return new RazorPagesInitializer(Configuration);
//            yield return new MvcInitializer(Configuration);
//        }
//    }
//}
