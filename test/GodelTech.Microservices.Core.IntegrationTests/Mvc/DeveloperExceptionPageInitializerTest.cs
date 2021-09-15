using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Contracts;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class DeveloperExceptionPageInitializerTest : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public DeveloperExceptionPageInitializerTest()
        {
            _fixture = new AppTestFixture();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(DeveloperExceptionPageInitializer initializer)
        {
            return _fixture
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder
                            .ConfigureServices(
                                services =>
                                {
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
                    }
                )
                .CreateClient();
        }

        [Fact]
        public async Task Configure_WhenIsDevelopmentEnvironment_Success()
        {
            // Arrange
            var initializer = new DeveloperExceptionPageInitializer();

            _fixture.SetEnvironment("Development");

            var client = CreateClient(initializer);

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
                new MediaTypeHeaderValue("text/plain"),
                result.Content.Headers.ContentType
            );

            Assert.Equal(
                await File.ReadAllTextAsync($"Documents/DeveloperExceptionPage.txt"),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_WhenIsNotDevelopmentEnvironment_Success()
        {
            // Arrange
            var initializer = new DeveloperExceptionPageInitializer();

            var client = CreateClient(initializer);

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