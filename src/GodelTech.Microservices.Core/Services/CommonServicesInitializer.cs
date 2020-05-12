using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Services
{
    public class CommonServicesInitializer : MicroserviceInitializerBase
    {
        public CommonServicesInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var correlationIdAccessor = new CorrelationIdAccessor();

            services.AddSingleton<ICorrelationIdAccessor>(correlationIdAccessor);
            services.AddSingleton<ICorrelationIdSetter>(correlationIdAccessor);

            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IZipService, ZipService>();
            services.AddSingleton<IStreamDataReader, StreamDataReader>();
            services.AddSingleton<ISha512HashCalculator, Sha512HashCalculator>();
        }
    }
}