using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.IntegrationTests.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GodelTech.Microservices.IntegrationTests.Mvc
{
    public class RazorPagesInitializerTests : IntegrationTestBase
    {
        [Fact]
        public async Task InvokeHomePage_WhenProviderConfigured_ShouldReturnExpectedString()
        {
            static IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
            {
                yield return new GenericInitializer(null, (app, env) => app.UseRouting());

                yield return new RazorPagesInitializer(configuration);
            }

            var client = CreateClient(CreateInitializers);

            var response = await client.GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.GetText().Should().Contain("Welcome to Razor Pages");
        }

        [Fact]
        public async Task InvokeHomePage_WhenInitializerProviderNotProvided_ShouldReturn404()
        {
            static IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
            {
                yield return new GenericInitializer(null, (app, env) => app.UseRouting());
            }

            var client = CreateClient(CreateInitializers);

            var response = await client.GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}