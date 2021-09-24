using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// Api initializer.
    /// </summary>
    public class ApiInitializer : IMicroserviceInitializer
    {
        private readonly Action<MvcOptions> _configureMvc;
        private readonly Action<JsonOptions> _configureJson;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiInitializer"/> class.
        /// </summary>
        /// <param name="configureMvc">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <param name="configureJson">An <see cref="Action{JsonOptions}"/> to configure the provided <see cref="JsonOptions"/>.</param>
        public ApiInitializer(
            Action<MvcOptions> configureMvc = null,
            Action<JsonOptions> configureJson = null)
        {
            _configureMvc = configureMvc;
            _configureJson = configureJson;
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(
                    options =>
                    {
                        ConfigureMvcOptions(options);

                        _configureMvc?.Invoke(options);
                    }
                )
                .AddJsonOptions(
                    options =>
                    {
                        ConfigureJsonOptions(options);

                        _configureJson?.Invoke(options);
                    }
                );
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
        }

        /// <summary>
        /// Configure MvcOptions.
        /// </summary>
        /// <param name="options">MvcOptions.</param>
        protected virtual void ConfigureMvcOptions(MvcOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

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
            if (options == null) throw new ArgumentNullException(nameof(options));

            // todo: a.solonoy: why such values?
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.IgnoreNullValues = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }
    }
}