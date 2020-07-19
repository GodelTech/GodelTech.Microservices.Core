using System;
using System.Collections.Generic;
using System.Net.Http;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.IntegrationTests.Utils;
using GodelTech.Microservices.Website;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected HttpClient CreateClient(Func<IConfiguration, IEnumerable<IMicroserviceInitializer>> intializerFactory)
        {
            IntegrationTestsStartup.InitializerFactory = intializerFactory;

            var factory = new IntegrationTestWebApplicationFactory<IntegrationTestsStartup>();

            return factory.CreateClient();
        }
    }
}