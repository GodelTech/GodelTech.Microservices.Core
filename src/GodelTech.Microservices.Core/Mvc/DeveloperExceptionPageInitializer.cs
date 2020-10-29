using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.Mvc
{
    public class DeveloperExceptionPageInitializer : MicroserviceInitializerBase
    {
        /// <summary>
        /// This option makes sense for UI application. REST API application should return
        /// HTTP status error code rather than render HTML page containing error information.
        /// </summary>
        public string ErrorHandlingPath { get; set; }

        public DeveloperExceptionPageInitializer(IConfiguration configuration) 
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
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ErrorHandlingPath))
                    app.UseExceptionHandler(ErrorHandlingPath);
            }
        }
    }
}
