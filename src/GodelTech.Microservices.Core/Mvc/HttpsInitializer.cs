using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.Mvc
{
    public class HttpsInitializer : MicroserviceInitializerBase
    {
        public HttpsInitializer(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) 
                throw new ArgumentNullException(nameof(app));
            if (env == null) 
                throw new ArgumentNullException(nameof(env));

            if (!env.IsDevelopment())
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            app.UseHttpsRedirection();
        }
    }
}