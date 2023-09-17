using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc;
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

        private bool _wasConfigureRequestResponseLoggingOptionsCalled;

        public RequestResponseLoggingMiddlewareInitializerTests(ITestOutputHelper output)
        {
            _fixture = new AppTestFixture
            {
                Output = output
            };
            _fixture.SetConfiguration(
                GetConfiguration(),
                new FakeRequestResponseLoggingMiddlewareInitializer(
                    () => _wasConfigureRequestResponseLoggingOptionsCalled = true
                )
            );
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
            Assert.True(_wasConfigureRequestResponseLoggingOptionsCalled);

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

            var requestLog = middlewareLogs[0].Message;

            Assert.Matches(
                new Regex(
                    "^" +
                    "Http Request Information:" + Environment.NewLine +
                    "TraceIdentifier: [0-9A-Z]{13}," +
                    "Method: POST," +
                    @"Url: http://localhost/fakes\?version=1," +
                    @"RemoteIP: \(null\)," +
                    @"RequestHeaders: {""Content-Type"":\[""application/json; charset=utf-8""\],""Content-Length"":\[""59""\],""Host"":\[""localhost""\]}" +
                    @",Body: {""serviceName"":""Test ServiceName"",""message"":""Test Message""}" +
                    "$"
                ),
                requestLog
            );

            var responseLog = middlewareLogs[1].Message;

            Assert.Matches(
                new Regex(
                    "^" +
                    "Http Response Information:" + Environment.NewLine +
                    "TraceIdentifier: [0-9A-Z]{13}," +
                    "StatusCode: 201," +
                    @"ReasonPhrase: \(null\)," +
                    "ResponseTimeMilliseconds: [0-9]{1,}," +
                    @"ResponseHeaders: {""Location"":\[""http://localhost/fakes/3""\],""Content-Type"":\[""application/json; charset=utf-8""\]}" +
                    @",Body: {""id"":3,""serviceName"":""Test ServiceName"",""message"":""Test Message"",""dictionary"":{},""emptyDictionary"":null,""intValue"":0,""nullableIntValue"":null,""status"":0}" +
                    "$"
                ),
                responseLog
            );

            Assert.Equal(
                requestLog
                    .Replace("Http Request Information:" + Environment.NewLine, string.Empty, StringComparison.InvariantCulture)
                    .Replace("TraceIdentifier: ", string.Empty, StringComparison.InvariantCulture)
                    [..13],
                responseLog
                    .Replace("Http Response Information:" + Environment.NewLine, string.Empty, StringComparison.InvariantCulture)
                    .Replace("TraceIdentifier: ", string.Empty, StringComparison.InvariantCulture)
                    [..13]
            );
        }
    }
}
