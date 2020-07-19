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
    public class MvcInitializerTests : IntegrationTestBase
    {
        [Fact]
        public async Task InvokeHomePage_WhenProviderConfigured_ShouldReturnExpectedString()
        {
            static IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
            {
                yield return new GenericInitializer((app, env) => app.UseRouting());

                yield return new MvcInitializer(configuration);
            }

            var client = CreateClient(CreateInitializers);

            var response = await client.GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.GetText().Should().Contain("Welcome to MVC");
        }

        [Fact]
        public async Task InvokeHomePage_WhenInitializerProviderNotProvided_ShouldReturn404()
        {
            static IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
            {
                yield return new GenericInitializer((app, env) => app.UseRouting());
            }

            var client = CreateClient(CreateInitializers);

            var response = await client.GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task InvokeRestApi_WhenInitializerProviderConfigured_ShouldReturnExpectedString()
        {
            static IEnumerable<IMicroserviceInitializer> CreateInitializers(IConfiguration configuration)
            {
                yield return new GenericInitializer((app, env) => app.UseRouting());

                yield return new ApiInitializer(configuration);
            }

            var client = CreateClient(CreateInitializers);

            var response = await client.GetAsync("/v1/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.GetText().Should().Be("Welcome to REST API");
        }
    }
}