using GodelTech.Microservices.Core.Mvc.Security;
using Microsoft.AspNetCore.Authorization;
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

            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IGuidFactory, GuidFactory>();
            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IPathService, PathService>();
            services.AddSingleton<IZipService, ZipService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IStreamDataReader, StreamDataReader>();
            services.AddSingleton<ISha512HashCalculator, Sha512HashCalculator>();
            services.AddSingleton<IContentTypeResolver, ContentTypeResolver>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITempFileFactory, TempFileFactory>();
            services.AddTransient<ISecurityContext, SecurityContext>();
        }
    }
}