using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.HealthChecks
{
    public sealed class HealthChecksInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public HealthChecksInitializerTests()
        {
            _fixture = new AppTestFixture();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(HealthChecksInitializer initializer)
        {
            return _fixture
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder
                            .ConfigureServices(
                                services =>
                                {
                                    initializer.ConfigureServices(services);
                                }
                            );

                        builder
                            .Configure(
                                (context, app) =>
                                {
                                    initializer.Configure(app, context.HostingEnvironment);
                                }
                            );
                    }
                )
                .CreateClient();
        }

        public static IEnumerable<object[]> SuccessMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new HealthChecksInitializer(),
                    new Uri("/health", UriKind.Relative)
                },
                new object[]
                {
                    new HealthChecksInitializer("/testPath"),
                    new Uri("/testPath", UriKind.Relative)
                }
            };

        [Theory]
        [MemberData(nameof(SuccessMemberData))]
        public async Task Configure_Success(
            HealthChecksInitializer initializer,
            Uri uri)
        {
            // Arrange
            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(uri);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), result.Content.Headers.ContentType);
            Assert.Matches(
                new Regex(@"{""status"":""Healthy"",""results"":\[\],""totalDuration"":[\d]{1,4}[.,][\d]{1,4}}"),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public void Configure_WithHealthCheckOptions_Success()
        {
            // Arrange
            var mvcOptionsActionInvoked = false;

            Action<HealthCheckOptions, IApplicationBuilder> configureHealthCheck =
                (_, _) =>
                {
                    mvcOptionsActionInvoked = true;
                };

            var initializer = new HealthChecksInitializer(configure: configureHealthCheck);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(mvcOptionsActionInvoked);
        }
    }
}