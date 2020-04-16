using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Subsys
{
    public interface ISubsysTask
    {
        void Execute(IConfiguration configuration, IApplicationBuilder app, IWebHostEnvironment env);
    }
}