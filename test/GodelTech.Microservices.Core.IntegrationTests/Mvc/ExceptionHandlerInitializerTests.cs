﻿using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class ExceptionHandlerInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public ExceptionHandlerInitializerTests()
        {
            _fixture = new AppTestFixture();
            _fixture.SetConfiguration(GetConfiguration(), new ExceptionHandlerInitializer());
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private static Action<IWebHostBuilder, IMicroserviceInitializer> GetConfiguration()
        {
            return (builder, initializer) =>
            {
                builder
                    .ConfigureServices(
                        services =>
                        {
                            services.AddAutoMapper(typeof(TestStartup).Assembly);

                            services.AddTransient<IFakeService, FakeService>();

                            initializer.ConfigureServices(services);

                            services.AddControllers();
                        }
                    );

                builder
                    .Configure(
                        (context, app) =>
                        {
                            initializer.Configure(app, context.HostingEnvironment);

                            app.UseRouting();

                            app.UseEndpoints(
                                endpoints =>
                                {
                                    endpoints.MapControllers();
                                }
                            );
                        }
                    );
            };
        }

        [Fact]
        public async Task Configure_WhenIsNotDevelopmentEnvironment_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes/argumentException",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal(
                new MediaTypeHeaderValue("application/problem+json")
                {
                    CharSet = "utf-8"
                },
                result.Content.Headers.ContentType
            );

            Assert.Matches(
                new Regex(
                    "^{" +
                    "\"type\":\"https://tools.ietf.org/html/rfc[0-9]{4}#section-(\\d+\\.)?(\\d+\\.)?(\\d+)\"," +
                    "\"title\":\"An error occurred while processing your request.\"," +
                    "\"status\":500," +
                    "\"traceId\":\"[0-9]{2}-[a-f0-9]{32}-[a-f0-9]{16}-[0-9]{2}\"" +
                    "}$"
                ),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_WhenIsDevelopmentEnvironment_Success()
        {
            // Arrange
            _fixture.SetEnvironment("Development");

            var client = _fixture.CreateClient();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => client.GetAsync(
                    new Uri(
                        "/fakes/argumentException",
                        UriKind.Relative
                    )
                )
            );

            Assert.Equal("Fake ArgumentException (Parameter 'name')", exception.Message);
            Assert.Equal("name", exception.ParamName);
        }
    }
}
