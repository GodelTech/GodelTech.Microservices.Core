using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.IntegrationTests
{
    public class AppTestFixture : WebApplicationFactory<TestStartup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(
                    x =>
                    {
                        x.UseStartup<TestStartup>()
                            .UseTestServer();
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