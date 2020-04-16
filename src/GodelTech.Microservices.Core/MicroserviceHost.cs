using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using GodelTech.Microservices.Core.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GodelTech.Microservices.Core
{
    public static class MicroserviceHost
    {
        public static IHostBuilder CreateHostBuilder<TStartup>(string[] args, IReadOnlyDictionary<string, string> environmentVariableMappings)
            where TStartup : class
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((context, logging) =>
                {
                    // Lines below are required to exclude default .NET Core logging providers
                    // These providers are excluded to make sure that logging format remains
                    // the same as in .NET Framework microservices.
                    logging.ClearProviders();
                })
                .UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.AddInMemoryCollectionFromEnvVariables(environmentVariableMappings);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
        }
    }
}
