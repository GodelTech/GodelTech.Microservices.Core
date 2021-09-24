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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class ApiInitializerTest : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public ApiInitializerTest()
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
                                    services.AddTransient<IFakeService, FakeService>();

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

        [Fact]
        public async Task Configure_Success()
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
                "[" +
                "{" +
                "\"id\":0," +
                "\"intValue\":0," +
                "\"status\":\"Default\"" +
                "}," +
                "{" +
                "\"id\":1," +
                "\"serviceName\":\"FakeService\"," +
                "\"message\":\"Test Message\"," +
                "\"dictionary\":" +
                "{" +
                "\"firstKey\":\"FirstValue\"," +
                "\"second Key\":\"Second Value\"," +
                "\"third key\":\"third value\"" +
                "}," +
                "\"intValue\":97," +
                "\"status\":\"Default\"" +
                "}," +
                "{" +
                "\"id\":2," +
                "\"intValue\":97," +
                "\"nullableIntValue\":3," +
                "\"status\":\"Other\"" +
                "}" +
                "]",
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public void Configure_WithMvcOptions_Success()
        {
            // Arrange
            var mvcOptionsActionInvoked = false;

            Action<MvcOptions> configureMvc =
                _ =>
                {
                    mvcOptionsActionInvoked = true;
                };

            var initializer = new ApiInitializer(configureMvc);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(mvcOptionsActionInvoked);
        }

        [Fact]
        public void Configure_WithJsonOptions_Success()
        {
            // Arrange
            var jsonOptionsActionInvoked = false;

            Action<JsonOptions> configureJson =
                _ =>
                {
                    jsonOptionsActionInvoked = true;
                };

            var initializer = new ApiInitializer(configureJson: configureJson);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(jsonOptionsActionInvoked);
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