using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.DataLayer.Specifications;

namespace GodelTech.Microservices.Core.DataLayer.Repositories
{
    public interface IRepository<T>
    {
        ValueTask<T> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> GetAsync(ISpecification<T> spec, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> projection, CancellationToken cancellationToken = default(CancellationToken));
        Task<T[]> ListAsync(ISpecification<T> spec, int? skip = null, int? take = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult[]> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> projection, int? skip = null, int? take = null, CancellationToken cancellationToken = default(CancellationToken));
        Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> ExistsAsync(ISpecification<T> spec, CancellationToken cancellationToken = default(CancellationToken));
    }
}