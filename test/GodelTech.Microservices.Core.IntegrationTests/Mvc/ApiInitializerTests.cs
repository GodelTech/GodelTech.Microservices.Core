using System;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
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

        private bool _wasConfigureResponseCachingOptionsCalled;
        private bool _wasConfigureMemoryCacheOptionsCalled;

        public ApiInitializerTests()
        {
            _fixture = new AppTestFixture();
            _fixture.SetConfiguration(
                GetConfiguration(),
                new FakeApiInitializer(
                    () => _wasConfigureResponseCachingOptionsCalled = true,
                    () => _wasConfigureMemoryCacheOptionsCalled = true
                )
            );
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
                    .ConfigureTestServices(
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
                            initializer.ConfigureEndpoints(app, context.HostingEnvironment);
                        }
                    );
            };
        }

        [Fact]
        public async Task Configure_WhenList_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/api/fakes",
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
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    $"/api/fakes/{id}",
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
        public async Task Configure_WhenResponseCache_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result1 = await client.GetAsync(
                new Uri(
                    "/api/fakes/responseCache?testKey=testValue",
                    UriKind.Relative
                )
            );

            var result2 = await client.GetAsync(
                new Uri(
                    "/api/fakes/responseCache?testKey=testValue",
                    UriKind.Relative
                )
            );

            var result3 = await client.GetAsync(
                new Uri(
                    "/api/fakes/responseCache?testKey=newTestValue",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.True(_wasConfigureResponseCachingOptionsCalled);

            Assert.Equal(HttpStatusCode.OK, result1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, result2.StatusCode);
            Assert.Equal(HttpStatusCode.OK, result3.StatusCode);

            Assert.Equal(
                await result1.Content.ReadAsStringAsync(),
                await result2.Content.ReadAsStringAsync()
            );
            Assert.NotEqual(
                await result2.Content.ReadAsStringAsync(),
                await result3.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_WhenMemoryCache_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            var memoryCache = _fixture.Services.GetRequiredService<IMemoryCache>();
            var hasCacheValue = memoryCache.TryGetValue("_Current_Guid", out Guid? cacheValue);
            Assert.False(hasCacheValue);
            Assert.Null(cacheValue);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/api/fakes/memoryCache",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.True(_wasConfigureMemoryCacheOptionsCalled);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                await result.Content.ReadFromJsonAsync<Guid>(),
                memoryCache.Get<Guid>("_Current_Guid")
            );
        }

        [Fact]
        public async Task Configure_HasFileTooLargeExceptionFilter()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/api/fakes/fileTooLargeException",
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
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/api/fakes/requestValidationException",
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
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/api/fakes/resourceNotFoundException",
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
