using System;
using System.Data;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.DataLayer.Dapper
{
    public abstract class DapperRepository
    {
        private readonly IDbConnectionFactory _factory;

        protected DapperRepository(IDbConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected async Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            using (var connection = _factory.Create())
            {
                connection.Open();

                return await execute(connection);
            }
        }

        protected async Task ExecuteAsync(Func<IDbConnection, Task> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            using (var connection = _factory.Create())
            {
                connection.Open();

                await execute(connection);
            }
        }
    }
}
