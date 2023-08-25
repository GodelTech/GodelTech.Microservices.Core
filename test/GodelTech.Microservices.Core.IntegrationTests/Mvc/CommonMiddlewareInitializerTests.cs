using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class CommonMiddlewareInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public CommonMiddlewareInitializerTests(ITestOutputHelper output)
        {
            _fixture = new AppTestFixture
            {
                Output = output
            };
            _fixture.SetConfiguration(GetConfiguration(), new CommonMiddlewareInitializer());
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private Action<IWebHostBuilder, IMicroserviceInitializer> GetConfiguration()
        {
            return (builder, initializer) =>
            {
                builder.ConfigureTestLogging(_fixture.Output, _fixture.TestLoggerContextAccessor);

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
        public async Task Configure_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.PostAsJsonAsync(
                new Uri(
                    "/fakes?version=1",
                    UriKind.Relative
                ),
                new FakePostModel
                {
                    Message = "Test Message",
                    ServiceName = "Test ServiceName"
                }
            );

            // Assert
            var logs = _fixture
                .TestLoggerContextAccessor
                .TestLoggerContext
                .Entries;

            var requestResponseLogs = logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.RequestResponseLogging.RequestResponseLoggingMiddleware"
                )
                .ToList();

            Assert.Equal(2, requestResponseLogs.Count);

            Assert.True(result.Headers.Contains("X-Correlation-ID"));
        }

        [Fact]
        public async Task Configure_WithArgumentException()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(
                () => client.PostAsJsonAsync(
                    new Uri(
                        "/fakes/argumentException?version=1",
                        UriKind.Relative
                    ),
                    new FakePostModel
                    {
                        Message = "Test Message",
                        ServiceName = "Test ServiceName"
                    }
                )
            );

            // Assert
            var logs = _fixture
                .TestLoggerContextAccessor
                .TestLoggerContext
                .Entries;

            Assert.Single(logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.RequestResponseLogging.RequestResponseLoggingMiddleware"
                )
                .ToList()
            );

            Assert.Single(
                logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.LogUncaughtErrors.LogUncaughtErrorsMiddleware"
                )
            );
        }
    }
}
