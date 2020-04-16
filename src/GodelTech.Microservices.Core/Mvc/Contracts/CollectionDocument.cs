using System;
using System.Collections.Generic;
using System.Linq;

namespace GodelTech.Microservices.Core.Mvc.Contracts
{
    public class CollectionDocument<T>
    {
        public T[] Items { get; }
        public int Count { get; }

        public CollectionDocument(int count, IEnumerable<T> items)
            : this(count, items.ToArray())
        {
        }

        public CollectionDocument(int count, params T[] items)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count = count;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}