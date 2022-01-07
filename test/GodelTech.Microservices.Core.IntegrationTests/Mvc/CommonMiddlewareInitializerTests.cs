using System;
using System.Linq;
using System.Net.Http;
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
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }

        private HttpClient CreateClient(IMicroserviceInitializer initializer)
        {
            return _fixture
                .WithWebHostBuilder(
                    builder =>
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
                    }
                )
                .CreateClient();
        }

        [Fact]
        public async Task Configure_Success()
        {
            // Arrange
            var initializer = new CommonMiddlewareInitializer();

            // Act
            var client = CreateClient(initializer);

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

            Assert.Equal(2, requestResponseLogs.Count); // check if RequestResponseLoggingMiddleware was initialized
            
            var hasCorrelationIdHeader = result.Headers.Contains(
                "X-Correlation-ID");

            Assert.True(hasCorrelationIdHeader); // check if CorrelationIdMiddlewareInitializer was initialized

            await Assert.ThrowsAsync<ArgumentException>(
                () => client.GetAsync(
                    new Uri(
                        "/fakes/argumentException",
                        UriKind.Relative
                    )
                )
            );

            Assert.Single(
            logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.LogUncaughtErrors.LogUncaughtErrorsMiddleware"
                )
            ); // check if LogUncaughtErrorsMiddleware was initialized
        }
    }
}