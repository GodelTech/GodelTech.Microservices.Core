using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class CorrelationIdMiddlewareInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public CorrelationIdMiddlewareInitializerTests()
        {
            _fixture = new AppTestFixture();
            _fixture.SetConfiguration(GetConfiguration(), new CorrelationIdMiddlewareInitializer());
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

        public static IEnumerable<object[]> ConfigureMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new Dictionary<string, string>(),
                    @"[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}"
                },
                new object[]
                {
                    new Dictionary<string, string>
                    {
                        { "firstKey", "FirstTestValue" }
                    },
                    @"[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}"
                },
                new object[]
                {
                    new Dictionary<string, string>
                    {
                        { "X-Correlation-ID", "00000000-0000-0000-0000-000000000002" }
                    },
                    "00000000-0000-0000-0000-000000000002"
                },
                new object[]
                {
                    new Dictionary<string, string>
                    {
                        { "firstKey", "FirstTestValue" },
                        { "X-Correlation-ID", "00000000-0000-0000-0000-000000000002" }
                    },
                    "00000000-0000-0000-0000-000000000002"
                }
            };

        [Theory]
        [MemberData(nameof(ConfigureMemberData))]
        public async Task Configure_Success(
            Dictionary<string, string> requestHeaders,
            string expectedCorrelationIdRegex)
        {
            if (requestHeaders == null) throw new ArgumentNullException(nameof(requestHeaders));

            // Arrange
            var client = _fixture.CreateClient();

            foreach (var requestHeader in requestHeaders)
            {
                client.DefaultRequestHeaders.Add(requestHeader.Key, requestHeader.Value);
            }

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/correlationId",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Matches(
                new Regex(
                    "^{" +
                    "\"correlationId\":\"" + expectedCorrelationIdRegex + "\"" +
                    "}$"
                ),
                await result.Content.ReadAsStringAsync()
            );

            Assert.Single(result.Headers);

            var hasCorrelationIdHeader = result.Headers.TryGetValues(
                "X-Correlation-ID",
                out var actualCorrelationIdValue
            );

            Assert.True(hasCorrelationIdHeader);
            Assert.Matches(
                new Regex(expectedCorrelationIdRegex),
                actualCorrelationIdValue.Aggregate((a, b) => a + b)
            );
        }
    }
}
