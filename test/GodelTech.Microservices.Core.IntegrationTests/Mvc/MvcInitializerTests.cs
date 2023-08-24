using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class MvcInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public MvcInitializerTests()
        {
            _fixture = new AppTestFixture();
            _fixture.SetConfiguration(GetConfiguration(), new FakeMvcInitializer());
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
                            services.AddAutoMapper(typeof(TestStartup).Assembly);

                            services.AddTransient<IFakeService, FakeService>();

                            services.Configure<RazorViewEngineOptions>(
                                options =>
                                {
                                    options
                                        .ViewLocationFormats
                                        .Clear();

                                    options
                                        .ViewLocationFormats
                                        .Add("/Fakes/Views/{1}/{0}" + RazorViewEngine.ViewExtension);

                                    options
                                        .ViewLocationFormats
                                        .Add("/Fakes/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                                }
                            );

                            services.Configure<MvcRazorRuntimeCompilationOptions>(
                                options =>
                                {
                                    options.FileProviders
                                        .Add(
                                            new EmbeddedFileProvider(
                                                typeof(TestStartup).Assembly
                                            )
                                        );
                                }
                            );

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
        public async Task Configure_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                await File.ReadAllTextAsync("Documents/HomeIndex.txt"),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Configure_WhenItem_Success(int id)
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    $"/Home/Details/{id}",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                await File.ReadAllTextAsync($"Documents/HomeDetails.{id}.txt"),
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
                    "/Home/ResponseCache?testKey=testValue",
                    UriKind.Relative
                )
            );

            var result2 = await client.GetAsync(
                new Uri(
                    "/Home/ResponseCache?testKey=testValue",
                    UriKind.Relative
                )
            );

            var result3 = await client.GetAsync(
                new Uri(
                    "/Home/ResponseCache?testKey=newTestValue",
                    UriKind.Relative
                )
            );

            // Assert
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
            var hasCacheValue = memoryCache.TryGetValue("_Current_DateTime", out DateTime? cacheValue);
            Assert.False(hasCacheValue);
            Assert.Null(cacheValue);

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/Home/MemoryCache",
                    UriKind.Relative
                )
            );

            // Assert
            cacheValue = memoryCache.Get<DateTime>("_Current_DateTime");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Matches(
                new Regex("<div>" + cacheValue + "</div>"),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Theory]
        [InlineData("/Home/Test", HttpStatusCode.NotFound, "")]
        [InlineData("/Home/TestAsync", HttpStatusCode.OK, "TestAsync Content")]
        public async Task Configure_WhenSuppressAsyncSuffixInActionNamesFalse_Success(
            string path,
            HttpStatusCode expectedStatusCode,
            string expectedContent)
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    path,
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(expectedStatusCode, result.StatusCode);
            Assert.Equal(
                expectedContent,
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_MapControllerRoute_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/Home/Route",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(
                "/Home/Details/123",
                await result.Content.ReadAsStringAsync()
            );
        }
    }
}
