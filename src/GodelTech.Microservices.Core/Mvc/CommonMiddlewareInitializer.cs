using System;
using GodelTech.Microservices.Core.Mvc.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Mvc
{
    public class CommonMiddlewareInitializer : MicroserviceInitializerBase
    {
        public CommonMiddlewareInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<LogUncaughtErrorsMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}