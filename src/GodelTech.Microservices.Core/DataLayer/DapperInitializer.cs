using GodelTech.Microservices.Core.DataLayer.Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.DataLayer
{
    public class DapperInitializer : MicroserviceInitializerBase
    {
        public string ConnectionStringName { get; set; } = "Default";

        public DapperInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddSqlServer(Configuration.GetConnectionString(ConnectionStringName), name: "Dapper");

            services.AddSingleton<IDbConnectionFactory>(x => new MsSqlConnectionFactory(
                Configuration.GetConnectionString(ConnectionStringName)));
        }
    }
}