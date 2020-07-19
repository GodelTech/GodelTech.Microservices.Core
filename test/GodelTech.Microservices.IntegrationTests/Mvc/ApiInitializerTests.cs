using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Website;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GodelTech.Microservices.IntegrationTests.Mvc
{
    public class ApiInitializerTests : IntegrationTestBase
    {
        public ApiInitializerTests(IntegrationTestWebApplicationFactory<IntegrationTestsStartup> factory) 
            : base(factory)
        {
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var client = Factory.CreateClient();

            var response = await client.GetAsync("/v1/users");

            response.EnsureSuccessStatusCode(); 
            response.Content.ReadAsStringAsync().GetAwaiter().GetResult().Should().Be("Hello World!");
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
        {
            yield return new GenericInitializer((app, env) => app.UseRouting());

            yield return new ApiInitializer(configuration);
        }
    }
}
