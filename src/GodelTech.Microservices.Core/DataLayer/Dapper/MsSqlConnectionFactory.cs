using System;
using System.Data;
using System.Data.SqlClient;

namespace GodelTech.Microservices.Core.DataLayer.Dapper
{
    public class MsSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public MsSqlConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}