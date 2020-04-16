using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Subsys
{
    public class SubsysInitializer : MicroserviceInitializerBase
    {
        private readonly ISubsysTask[] _tasks;

        public string EnvName { get; set; } = "Subsys";

        public SubsysInitializer(IConfiguration configuration, params ISubsysTask[] tasks)
            : base(configuration)
        {
            _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.Equals(EnvName, StringComparison.OrdinalIgnoreCase))
                Array.ForEach(_tasks, x => x.Execute(Configuration, app, env));
        }
    }
}