using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Website;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GodelTech.Microservices.IntegrationTests.Mvc
{
    public class ApiInitializerTests : IClassFixture<WebApplicationFactory<IntegrationTestsStartup>>
    {
        private readonly WebApplicationFactory<IntegrationTestsStartup> _factory;

        public ApiInitializerTests(WebApplicationFactory<IntegrationTestsStartup> factory)
        {
            _factory = factory;

            Program.UseIntegrationTestsStartup = true;
            IntegrationTestsStartup.InitializerFactory = CreateInitializers;
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/v1/users");

            response.EnsureSuccessStatusCode(); 
            response.Content.ReadAsStringAsync().GetAwaiter().GetResult().Should().Be("Hello World!");
        }

        private IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
        {
            yield return new GenericInitializer((app, env) => app.UseRouting());

            yield return new ApiInitializer(configuration);
        }
    }
}
