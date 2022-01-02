using System.IO;
using GodelTech.XUnit.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace GodelTech.Microservices.Core.IntegrationTests
{
    public class AppTestFixture : WebApplicationFactory<TestStartup>
    {
        private string _environment;

        public ITestOutputHelper Output { get; set; }

        public ITestLoggerContextAccessor TestLoggerContextAccessor { get; } = new TestLoggerContextAccessor();

        public void SetEnvironment(string environment)
        {
            _environment = environment;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(
                    x =>
                    {
                        x.UseStartup<TestStartup>()
                            .UseTestServer();

                        if (!string.IsNullOrWhiteSpace(_environment))
                        {
                            x.UseEnvironment(_environment);
                        }
                    }
                );

            return builder;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());

            return base.CreateHost(builder);
        }
    }
}