using System;
using System.Linq;
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
using Xunit.Abstractions;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class RequestResponseLoggingMiddlewareInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public RequestResponseLoggingMiddlewareInitializerTests(ITestOutputHelper output)
        {
            _fixture = new AppTestFixture
            {
                Output = output
            };
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(RequestResponseLoggingMiddlewareInitializer initializer)
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
            var initializer = new RequestResponseLoggingMiddlewareInitializer();

            var client = CreateClient(initializer);

            // Act 
            var result = await client.GetAsync(
                new Uri(
                    "/fakes/1",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var logs = _fixture
                .TestLoggerContextAccessor
                .TestLoggerContext
                .Entries;

            var controllerLog = logs.Where(
                x =>
                    x.CategoryName ==
                    "GodelTech.Microservices.Core.Mvc.RequestResponseLogging.RequestResponseLoggingMiddleware");

            Assert.Equal(2, controllerLog.Count());

            Assert.Equal("Http Request Information:\r\nSchema: http Host: localhost Path: /fakes/1 QueryString: ", controllerLog.First().Message);
        }
    }
}