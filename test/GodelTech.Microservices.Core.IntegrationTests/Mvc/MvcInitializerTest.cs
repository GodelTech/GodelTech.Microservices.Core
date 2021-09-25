﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers;
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
    public sealed class MvcInitializerTest : IDisposable
    {
        private readonly AppTestFixture _fixture;

        public MvcInitializerTest()
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
                        builder.ConfigureAppConfiguration(
                            (context, _) =>
                            {
                                context.HostingEnvironment.ApplicationName =
                                    typeof(HomeController).Assembly.GetName().Name;
                            }
                        );

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
                                                        typeof(HomeController).Assembly
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
                        .AddApplicationPart(typeof(HomeController).Assembly)
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