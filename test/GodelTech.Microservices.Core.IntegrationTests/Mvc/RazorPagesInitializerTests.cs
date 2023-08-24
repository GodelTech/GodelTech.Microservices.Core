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
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace GodelTech.Microservices.Core.IntegrationTests.Mvc
{
    public sealed class RazorPagesInitializerTests : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public RazorPagesInitializerTests()
        {
            _fixture = new AppTestFixture();
            _fixture.SetConfiguration(GetConfiguration(), new FakeRazorPagesInitializer());
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
                            services.AddTransient<IFakeService, FakeService>();

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

                            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Fakes/Pages");

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
                await File.ReadAllTextAsync("Documents/PagesIndex.txt"),
                await result.Content.ReadAsStringAsync()
            );
        }

        [Fact]
        public async Task Configure_WhenResponseCache_Success()
        {
            // Arrange
            var client = _fixture.CreateClient();

            // Act
            var result = await client.GetAsync(
                new Uri(
                    "/ResponseCache",
                    UriKind.Relative
                )
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
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
                    "/MemoryCache",
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
    }
}
