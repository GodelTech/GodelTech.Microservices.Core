using System;
using System.Linq;
using System.Net;
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
            var initializer = new RequestResponseLoggingMiddlewareInitializer();

            var client = CreateClient(initializer);

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
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            Assert.Equal("http://localhost/fakes/3", result.Headers.Location?.AbsoluteUri);

            var resultValue = await result.Content.ReadFromJsonAsync<FakeModel>();
            Assert.NotNull(resultValue);
            Assert.Equal(3, resultValue.Id);
            Assert.Equal("Test Message", resultValue.Message);
            Assert.Equal("Test ServiceName", resultValue.ServiceName);

            var logs = _fixture
                .TestLoggerContextAccessor
                .TestLoggerContext
                .Entries;

            var middlewareLogs = logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.RequestResponseLogging.RequestResponseLoggingMiddleware"
                )
                .ToList();

            Assert.Equal(2, middlewareLogs.Count);

            Assert.Equal(
                $"Http Request Information:{Environment.NewLine}" +
                "Method: POST," +
                "Url: http://localhost/fakes?version=1," +
                "RemoteIP: ," +
                "RequestHeaders: [Content-Type, application/json; charset=utf-8],[Content-Length, 59],[Host, localhost]",
                middlewareLogs.First().Message
            );
        }
    }
}