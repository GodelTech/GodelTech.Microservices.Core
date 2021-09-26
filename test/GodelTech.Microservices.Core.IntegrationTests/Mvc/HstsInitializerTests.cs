using System;
using System.Net;
using System.Net.Http;
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
    public sealed class HstsInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public HstsInitializerTests()
        {
            _fixture = new AppTestFixture();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(HstsInitializer initializer)
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

                                    services.AddHsts(
                                        options =>
                                        {
                                            options.ExcludedHosts.Clear();
                                        }
                                    );

                                    services.AddHttpsRedirection(
                                        options =>
                                        {
                                            options.HttpsPort = 5001;
                                        }
                                    );

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
        public async Task Configure_WhenIsNotDevelopmentEnvironment_Success()
        {
            // Arrange
            var initializer = new HstsInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Headers);
            Assert.Equal(
                new string[]
                {
                    "max-age=2592000"
                },
                result.Headers.GetValues("Strict-Transport-Security")
            );
            Assert.Equal(
                new Uri("https://localhost:5001/fakes"),
                result.RequestMessage?.RequestUri
            );
        }

        [Fact]
        public async Task Configure_WhenIsDevelopmentEnvironment_Success()
        {
            // Arrange
            var initializer = new HstsInitializer();

            _fixture.SetEnvironment("Development");

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Empty(result.Headers);
            Assert.Equal(
                new Uri("https://localhost:5001/fakes"),
                result.RequestMessage?.RequestUri
            );
        }
    }
}