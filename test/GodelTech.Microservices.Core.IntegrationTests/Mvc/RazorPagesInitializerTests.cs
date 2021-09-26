using System;
using System.Net.Http;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
    }
}