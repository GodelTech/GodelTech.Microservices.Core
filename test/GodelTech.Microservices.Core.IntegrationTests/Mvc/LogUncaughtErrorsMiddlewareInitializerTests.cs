using System;
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
            _fixture = new AppTestFixture();
            _fixture.Output = output;
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

            var controllerLog = Assert.Single(
                logs.Where(
                    x =>
                        x.CategoryName ==
                        "GodelTech.Microservices.Core.Mvc.LogUncaughtErrors.LogUncaughtErrorsMiddleware"
                )
            );
            Assert.Equal(
                "Action=LogUncaughtErrors, Message=Uncaught error:Fake ArgumentException (Parameter 'name'), Method=GET, RequestUri=http://localhost/fakes/argumentException",
                controllerLog.Message
            );
        }
    }
}