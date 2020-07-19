using System;
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
    public class ApiInitializer : MicroserviceInitializerBase
    {
        protected virtual CompatibilityVersion CompatibilityVersion => CompatibilityVersion.Version_3_0;

        public ApiInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddControllers(ConfigureMvcOptions)
                .AddJsonOptions(ConfigureJsonOptions)
                .SetCompatibilityVersion(CompatibilityVersion);
        }

        protected virtual void ConfigureJsonOptions(JsonOptions options)
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.IgnoreNullValues = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
            options.SuppressAsyncSuffixInActionNames = false;

            options.Filters.Add(new BadRequestOnExceptionAttribute(typeof(RequestValidationException)));
            options.Filters.Add(new NotFoundOnExceptionAttribute(typeof(ResourceNotFoundException)));
            options.Filters.Add(new HttpStatusCodeOnExceptionAttribute(413, typeof(FileTooLargeExceptionException)));
        }
    }
}