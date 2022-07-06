using System;
using System.Globalization;
using System.Linq;
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
    public sealed class LogUncaughtErrorsMiddlewareInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public LogUncaughtErrorsMiddlewareInitializerTests(ITestOutputHelper output)
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

        private HttpClient CreateClient(LogUncaughtErrorsMiddlewareInitializer initializer)
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
            var initializer = new LogUncaughtErrorsMiddlewareInitializer();

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

            var logs = _fixture
                .TestLoggerContextAccessor
                .TestLoggerContext
                .Entries;

            var middlewareLog = Assert.Single(
                logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.LogUncaughtErrors.LogUncaughtErrorsMiddleware"
                )
            );
            Assert.Equal(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Action={0}," +
                    "Message=Uncaught error:{1}," +
                    "Method={2}," +
                    "RequestUri={3}",
                    "LogUncaughtErrors",
                    "Fake ArgumentException (Parameter 'name')",
                    "GET",
                    "http://localhost/fakes/argumentException"
                ),
                middlewareLog.Message
            );
        }
    }
}
