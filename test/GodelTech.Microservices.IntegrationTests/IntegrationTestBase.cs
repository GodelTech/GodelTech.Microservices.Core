using System.Collections.Generic;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Website;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GodelTech.Microservices.IntegrationTests
{
    public abstract class IntegrationTestBase: IClassFixture<IntegrationTestWebApplicationFactory<IntegrationTestsStartup>>
    {
        protected IntegrationTestWebApplicationFactory<IntegrationTestsStartup> Factory { get; }

        protected IntegrationTestBase(IntegrationTestWebApplicationFactory<IntegrationTestsStartup> factory)
        {
            Factory = factory;

            IntegrationTestsStartup.InitializerFactory = CreateInitializers;
        }

        protected virtual IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
        {
            yield break;
        }
    }
}