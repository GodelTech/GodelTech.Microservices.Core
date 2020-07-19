using System;
using System.Collections.Generic;
using GodelTech.Microservices.Core;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Website
{
    /// <summary>
    /// This class is used by integration tests only. It's located in the same assembly as controllers.
    /// As result there is no need to use workarounds to load controllers, views and pages correctly.
    /// Tests must be executed sequentially in order to avoid concurrency issues when InitializerFactory is set by different threads.
    /// Detailed information can be found here: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    public class IntegrationTestsStartup : MicroserviceStartup
    {
        public static Func<IConfiguration, IEnumerable<IMicroserviceInitializer>> InitializerFactory;

        public IntegrationTestsStartup(IConfiguration configuration) 
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            return InitializerFactory(Configuration);
        }
    }
}