using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.DataLayer.Specifications;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Microservices.Core.DataLayer.Repositories
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected DbContext DbContext { get; }

        public EntityFrameworkRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ValueTask<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return DbContext.Set<TEntity>().FindAsync(new[] { id }, cancellationToken);
        }

        public virtual Task<bool> ExistsAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var query = DbContext.Set<TEntity>().AsQueryable();

            query = spec.AddPredicates(query);

            return query.AnyAsync(cancellationToken);
        }

        public virtual Task<TEntity> GetAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildGetQuery(spec).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> projection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (projection == null)
                throw new ArgumentNullException(nameof(projection));

            return BuildGetQuery(spec)
                .Select(projection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TEntity[]> ListAsync(ISpecification<TEntity> spec, int? skip = null, int? take = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildListQuery(spec, skip, take).ToArrayAsync(cancellationToken);
        }

        public Task<TResult[]> ListAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> projection, int? skip = null, int? take = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (projection == null)
                throw new ArgumentNullException(nameof(projection));

            return BuildListQuery(spec, skip, take)
                .Select(projection)
                .ToArrayAsync(cancellationToken);
        }

        public Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var query = spec.AddPredicates(DbContext.Set<TEntity>().AsQueryable());

            return query.CountAsync(cancellationToken);
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbContext.Set<TEntity>().Add(entity);
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbContext.Entry(entity).State = EntityState.Modified;

            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbContext.Set<TEntity>().Remove(entity);

            return DbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<TEntity> BuildGetQuery(ISpecification<TEntity> spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var query = DbContext.Set<TEntity>().AsQueryable();

            query = spec.AddPredicates(query);
            query = spec.AddSorting(query);
            query = spec.AddEagerFetching(query);

            return query;
        }

        private IQueryable<TEntity> BuildListQuery(ISpecification<TEntity> spec, int? skip, int? take)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            if ((skip ?? 0) < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));

            if (take.HasValue && take <= 0)
                throw new ArgumentOutOfRangeException(nameof(take));

            var query = BuildGetQuery(spec);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return query;
        }
    }
}
