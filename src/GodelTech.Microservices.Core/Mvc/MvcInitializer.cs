using System.Text.Json;
using System.Text.Json.Serialization;
using GodelTech.Microservices.Core.Exceptions;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.Mvc
{
    public class MvcInitializer : MicroserviceInitializerBase
    {
        protected virtual CompatibilityVersion CompatibilityVersion => CompatibilityVersion.Version_3_0;

        public MvcInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(ConfigureMvcOptions)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion);
        }

        protected virtual void ConfigureMvcOptions(MvcOptions x)
        {
            x.SuppressAsyncSuffixInActionNames = false;
            x.Filters.Add(new BadRequestOnExceptionAttribute(typeof(RequestValidationException)));
            x.Filters.Add(new NotFoundOnExceptionAttribute(typeof(ResourceNotFoundException)));
            x.Filters.Add(new HttpStatusCodeOnExceptionAttribute(413, typeof(FileTooLargeExceptionException)));
        }
    }
}