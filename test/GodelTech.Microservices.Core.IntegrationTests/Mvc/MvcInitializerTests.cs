using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
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
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(MvcInitializer initializer)
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
            Action<IMvcBuilder> configureBuilder =
                builder =>
                {
                    builder
                        .AddApplicationPart(typeof(TestStartup).Assembly)
                        .AddRazorRuntimeCompilation();
                };

            var initializer = new MvcInitializer(configureBuilder: configureBuilder);

            var client = CreateClient(initializer);

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
            Action<IMvcBuilder> configureBuilder =
                builder =>
                {
                    builder
                        .AddApplicationPart(typeof(TestStartup).Assembly)
                        .AddRazorRuntimeCompilation();
                };

            var initializer = new MvcInitializer(configureBuilder: configureBuilder);

            var client = CreateClient(initializer);

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

        [Theory]
        [InlineData("/Home/Test", HttpStatusCode.NotFound, "")]
        [InlineData("/Home/TestAsync", HttpStatusCode.OK, "TestAsync Content")]
        public async Task Configure_WhenSuppressAsyncSuffixInActionNamesFalse_Success(
            string path,
            HttpStatusCode expectedStatusCode,
            string expectedContent)
        {
            // Arrange
            Action<IMvcBuilder> configureBuilder =
                builder =>
                {
                    builder
                        .AddApplicationPart(typeof(TestStartup).Assembly)
                        .AddRazorRuntimeCompilation();
                };

            var initializer = new MvcInitializer(configureBuilder: configureBuilder);

            var client = CreateClient(initializer);

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
        public void Configure_WithMvcOptions_Success()
        {
            // Arrange
            var mvcOptionsActionInvoked = false;

            Action<MvcOptions> configureMvc =
                _ =>
                {
                    mvcOptionsActionInvoked = true;
                };

            var initializer = new MvcInitializer(configureMvc);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(mvcOptionsActionInvoked);
        }

        [Fact]
        public void Configure_WithMvcBuilder_Success()
        {
            // Arrange
            var mvcBuilderActionInvoked = false;

            Action<IMvcBuilder> configureBuilder =
                _ =>
                {
                    mvcBuilderActionInvoked = true;
                };

            var initializer = new MvcInitializer(configureBuilder: configureBuilder);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(mvcBuilderActionInvoked);
        }
    }
}