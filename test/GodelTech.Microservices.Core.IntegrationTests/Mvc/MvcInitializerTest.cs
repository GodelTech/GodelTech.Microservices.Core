using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
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

                                    initializer.ConfigureServices(services);
                                }
                            );

                        builder
                            .Configure(
                                (context, app) =>
                                {
                                    app.UseDefaultFiles();
                                    app.UseStaticFiles();

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
            var initializer = new MvcInitializer();

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

            var initializer = new MvcInitializer(configureMvc);

            // Act
            CreateClient(initializer);

            // Assert
            Assert.True(mvcOptionsActionInvoked);
        }
    }
}