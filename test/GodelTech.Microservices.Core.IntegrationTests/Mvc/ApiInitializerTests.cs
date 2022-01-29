using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class ApiInitializerTests : IDisposable
    {
        private static readonly string[] FakeJsonStrings = new string[]
        {
            "{" +
            "\"id\":0," +
            "\"dictionary\":{}," +
            "\"intValue\":0," +
            "\"status\":\"Default\"" +
            "}",

            "{" +
            "\"id\":1," +
            "\"serviceName\":\"FakeService\"," +
            "\"message\":\"Test Message\"," +
            "\"dictionary\":" +
            "{" +
            "\"firstKey\":\"FirstValue\"," +
            "\"second Key\":\"Second Value\"," +
            "\"third key lowercase\":\"third value lowercase\"" +
            "}," +
            "\"intValue\":97," +
            "\"status\":\"Default\"" +
            "}",

            "{" +
            "\"id\":2," +
            "\"dictionary\":{}," +
            "\"intValue\":97," +
            "\"nullableIntValue\":3," +
            "\"status\":\"Other\"" +
            "}"
        };
        
        private readonly AppTestFixture _fixture;

        public ApiInitializerTests()
        {
            _fixture = new AppTestFixture();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(ApiInitializer initializer)
        {
            return _fixture
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder
                            .ConfigureServices(
                                services =>
                                {
                                    services.AddAutoMapper(typeof(TestStartup).Assembly);

                                    services.AddTransient<IFakeService, FakeService>();

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

        [Fact]
        public async Task Configure_WhenList_Success()
        {
            // Arrange
            var initializer = new ApiInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                "[" + string.Join(',', FakeJsonStrings) + "]",
                await result.Content.ReadAsStringAsync()
            );
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public async Task Configure_WhenItem_Success(
            int id,
            int expectedFakeJsonStringsIndex)
        {
            // Arrange
            var initializer = new ApiInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    $"/fakes/{id}",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                FakeJsonStrings[expectedFakeJsonStringsIndex],
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_HasFileTooLargeExceptionFilter()
        {
            // Arrange
            var initializer = new ApiInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes/fileTooLargeException",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.RequestEntityTooLarge, result.StatusCode);

            Assert.Equal(
                "{\"errorMessage\":\"Exception of type 'GodelTech.Microservices.Core.FileTooLargeException' was thrown.\"}",
                await result.Content.ReadAsStringAsync()
            );

            var stream = await result.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var model = await result.Content.ReadFromJsonAsync<ExceptionFilterResultModel>();

            Assert.NotNull(model);
            Assert.Equal(
                "Exception of type 'GodelTech.Microservices.Core.FileTooLargeException' was thrown.",
                model.ErrorMessage
            );
        }

        [Fact]
        public async Task Configure_HasRequestValidationExceptionFilter()
        {
            // Arrange
            var initializer = new ApiInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes/requestValidationException",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            Assert.Equal(
                "{\"errorMessage\":\"Exception of type 'GodelTech.Microservices.Core.RequestValidationException' was thrown.\"}",
                await result.Content.ReadAsStringAsync()
            );

            var stream = await result.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var model = await result.Content.ReadFromJsonAsync<ExceptionFilterResultModel>();

            Assert.NotNull(model);
            Assert.Equal(
                "Exception of type 'GodelTech.Microservices.Core.RequestValidationException' was thrown.",
                model.ErrorMessage
            );
        }

        [Fact]
        public async Task Configure_HasResourceNotFoundExceptionFilter()
        {
            // Arrange
            var initializer = new ApiInitializer();

            var client = CreateClient(initializer);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/fakes/resourceNotFoundException",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            Assert.Equal(
                "{\"errorMessage\":\"Exception of type 'GodelTech.Microservices.Core.ResourceNotFoundException' was thrown.\"}",
                await result.Content.ReadAsStringAsync()
            );

            var stream = await result.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var model = await result.Content.ReadFromJsonAsync<ExceptionFilterResultModel>();

            Assert.NotNull(model);
            Assert.Equal(
                "Exception of type 'GodelTech.Microservices.Core.ResourceNotFoundException' was thrown.",
                model.ErrorMessage
            );
        }
    }
}