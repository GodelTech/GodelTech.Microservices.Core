using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Website
{
    public class Program
    {
        public static bool UseIntegrationTestsStartup = false;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (UseIntegrationTestsStartup)
                        webBuilder.UseStartup<IntegrationTestsStartup>();
                    else
                        webBuilder.UseStartup<Startup>();
                });
    }
}
