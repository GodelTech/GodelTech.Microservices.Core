using GodelTech.Microservices.Core.DataLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodelTech.Microservices.Core.DataLayer
{
    public class EntityFrameworkInitilizer<TDatabaseContext> : MicroserviceInitializerBase
        where TDatabaseContext : DbContext
    {
        public string ConnectionStringName { get; set; } = "Default";

        public EntityFrameworkInitilizer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<TDatabaseContext>();
            context.Database.Migrate();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddSqlServer(Configuration.GetConnectionString(ConnectionStringName), name: "EntityFramework");

            services.AddDbContext<TDatabaseContext>((p, options) =>
            {
                options
                    .UseSqlServer(
                        Configuration.GetConnectionString(ConnectionStringName),
                        x => x.EnableRetryOnFailure())
                    .EnableSensitiveDataLogging(p.GetService<IHostEnvironment>().IsDevelopment());
            });

            services.AddTransient<DbContext>(x => x.GetRequiredService<TDatabaseContext>());
            services.AddTransient(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));
        }
    }
}