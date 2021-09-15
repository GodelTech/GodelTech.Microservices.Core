﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// ExceptionHandler initializer.
    /// </summary>
    public class ExceptionHandlerInitializer : IMicroserviceInitializer
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeveloperExceptionPageInitializer"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        public ExceptionHandlerInitializer(string path = "/Error")
        {
            _path = path;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler(_path);
            }
        }
    }
}