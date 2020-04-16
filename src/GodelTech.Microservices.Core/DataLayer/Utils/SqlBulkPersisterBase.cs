using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FastMember;

namespace GodelTech.Microservices.Core.DataLayer.Utils
{
    public abstract class SqlBulkPersisterBase<TEntity>
    {
        private int _batchSize = 10000;

        protected abstract string DestinationTableName { get; }
        protected bool EnableStreaming { get; set; }
        protected string ConnectionString { get; }

        protected int BatchSize
        {
            get => _batchSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _batchSize = value;
            }
        }

        protected SqlBulkPersisterBase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            ConnectionString = connectionString;
        }

        public virtual async Task SaveAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using (var connection = new SqlConnection(ConnectionString))
            using (var bcp = new SqlBulkCopy(connection))
            using (var reader = CreateReader(entities))
            {
                connection.Open();

                bcp.DestinationTableName = DestinationTableName;
                bcp.BatchSize = BatchSize;
                bcp.EnableStreaming = EnableStreaming;

                await bcp.WriteToServerAsync(reader);
            }
        }

        protected abstract string[] GetPropertiesToPersist();

        private DbDataReader CreateReader(IEnumerable<TEntity> issues)
        {
            return ObjectReader.Create(
                issues,
                GetPropertiesToPersist());
        }
    }
}
