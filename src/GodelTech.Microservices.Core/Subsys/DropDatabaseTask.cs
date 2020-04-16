using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Subsys
{
    public class DropDatabaseTask : ISubsysTask
    {
        public string ConnectionStringName { get; set; } = "Default";

        public void Execute(
            IConfiguration configuration, 
            IApplicationBuilder app, 
            IWebHostEnvironment env)
        {
            using var connection = new SqlConnection(GetMasterConnectionString(configuration));

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = CreateDropDbSql(configuration);

            command.ExecuteNonQuery();
        }

        private string CreateDropDbSql(IConfiguration configuration)
        {
            var builder = new SqlConnectionStringBuilder(configuration.GetConnectionString(ConnectionStringName));

            return string.Format(@"
            IF db_id('{0}') IS NOT NULL 
            BEGIN 
                ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{0}]
            END", builder.InitialCatalog);
        }

        private string GetMasterConnectionString(IConfiguration configuration)
        {
            var builder = new SqlConnectionStringBuilder(configuration.GetConnectionString(ConnectionStringName))
            {
                InitialCatalog = "master"
            };

            return builder.ToString();
        }
    }
}