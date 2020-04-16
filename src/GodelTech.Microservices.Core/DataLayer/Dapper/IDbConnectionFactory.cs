using System.Data;

namespace GodelTech.Microservices.Core.DataLayer.Dapper
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}