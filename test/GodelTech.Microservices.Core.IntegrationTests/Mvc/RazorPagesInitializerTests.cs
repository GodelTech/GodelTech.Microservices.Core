using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Mvc;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        private HttpClient CreateClient(RazorPagesInitializer initializer)
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
                    }
                )
                .CreateClient();
        }

        [Fact]
        public async Task Configure_Success()
        {
            // Arrange
            var initializer = new FakeRazorPagesInitializer();

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
                await File.ReadAllTextAsync("Documents/PagesIndex.txt"),
                await result.Content.ReadAsStringAsync()
            );
        }
    }
}
