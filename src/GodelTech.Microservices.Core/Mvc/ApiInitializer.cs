using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// Api initializer.
    /// </summary>
    public class ApiInitializer : IMicroserviceInitializer
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching(
                options =>
                {
                    ConfigureResponseCachingOptions(options);
                }
            );

            services.AddMemoryCache(
                options =>
                {
                    ConfigureMemoryCacheOptions(options);
                }
            );

            services
                .AddControllers(
                    options =>
                    {
                        ConfigureMvcOptions(options);
                    }
                )
                .AddJsonOptions(
                    options =>
                    {
                        ConfigureJsonOptions(options);
                    }
                );
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();
        }

        /// <inheritdoc />
        public virtual void ConfigureEndpoints(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
        }

        /// <summary>
        /// Configure ResponseCachingOptions.
        /// </summary>
        /// <param name="options">ResponseCachingOptions.</param>
        protected virtual void ConfigureResponseCachingOptions(ResponseCachingOptions options)
        {

        }

        /// <summary>
        /// Configure MemoryCacheOptions.
        /// </summary>
        /// <param name="options">MemoryCacheOptions.</param>
        protected virtual void ConfigureMemoryCacheOptions(MemoryCacheOptions options)
        {

        }

        /// <summary>
        /// Configure MvcOptions.
        /// </summary>
        /// <param name="options">MvcOptions.</param>
        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.Filters.Add(
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.RequestEntityTooLarge,
                    typeof(FileTooLargeException)
                )
            );
            options.Filters.Add(
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.BadRequest,
                    typeof(RequestValidationException)
                )
            );
            options.Filters.Add(
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.NotFound,
                    typeof(ResourceNotFoundException)
                )
            );
        }

        /// <summary>
        /// Configure JsonOptions.
        /// </summary>
        /// <param name="options">JsonOptions.</param>
        protected virtual void ConfigureJsonOptions(JsonOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
