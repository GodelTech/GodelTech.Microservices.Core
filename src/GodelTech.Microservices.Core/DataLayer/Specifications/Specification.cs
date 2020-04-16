using System;
using System.Linq;

namespace GodelTech.Microservices.Core.DataLayer.Specifications
{
    public class Specification<T> : ISpecification<T>
    {
        public virtual IQueryable<T> AddEagerFetching(IQueryable<T> query)
        {
            return query ?? throw new ArgumentNullException(nameof(query));
        }

        public virtual IQueryable<T> AddPredicates(IQueryable<T> query)
        {
            return query ?? throw new ArgumentNullException(nameof(query));
        }

        public virtual IQueryable<T> AddSorting(IQueryable<T> query)
        {
            return query ?? throw new ArgumentNullException(nameof(query));
        }
    }
}