using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.HealthChecks
{
    public sealed class HealthCheckInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public HealthCheckInitializerTests()
        {
            _fixture = new AppTestFixture();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(HealthCheckInitializer initializer)
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
                                    app.UseRouting();

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
                    new HealthCheckInitializer(),
                    new Uri("/health", UriKind.Relative)
                },
                new object[]
                {
                    new HealthCheckInitializer("/testPath"),
                    new Uri("/testPath", UriKind.Relative)
                }
            };

        [Theory]
        [MemberData(nameof(SuccessMemberData))]
        public async Task Configure_Success(
            HealthCheckInitializer initializer,
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
                new Regex(
                    "^{" +
                    "\"status\":\"Healthy\"," +
                    "\"results\":[[]]," +
                    "\"totalDuration\":[\\d]{1,4}[.,][\\d]{1,4}" +
                    "}$"
                ),
                await result.Content.ReadAsStringAsync()
            );

            var stream = await result.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var model = await result.Content
                .ReadFromJsonAsync<HealthCheckResponseModel>(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    }
                );

            Assert.NotNull(model);
            Assert.Equal(HealthStatus.Healthy, model.Status);
            Assert.Empty(model.Results);
            Assert.True(model.TotalDuration > 0);
        }
    }
}
