using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.DataLayer.Utils
{
    public abstract class StreamProviderBase
    {
        protected string ConnectionString { get; }

        protected StreamProviderBase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            ConnectionString = connectionString;
        }

        protected async Task<Stream> GetStreamAsync(string sql, Action<SqlParameterCollection> configureParams)
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(sql, connection);

            configureParams(command.Parameters);

            var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess);
            if (!await reader.ReadAsync())
                return Stream.Null;

            var stream = reader.GetStream(0);

            return new DbStreamAdapter(stream, new DisposableAction(() =>
            {
                reader.Dispose();
                connection.Dispose();
            }));
        }
    }
}