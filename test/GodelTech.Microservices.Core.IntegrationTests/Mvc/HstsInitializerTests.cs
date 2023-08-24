using System;
using System.Net;
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
            _fixture.SetConfiguration(GetConfiguration(), new HstsInitializer());
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
            _fixture.SetEnvironment("Development");

            var client = _fixture.CreateClient();

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
