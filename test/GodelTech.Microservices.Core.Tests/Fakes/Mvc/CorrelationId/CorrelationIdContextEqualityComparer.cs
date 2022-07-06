using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GodelTech.Microservices.Core.Mvc.CorrelationId;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc.CorrelationId
{
    public class CorrelationIdContextEqualityComparer : IEqualityComparer<CorrelationIdContext>
    {
        public bool Equals(CorrelationIdContext x, CorrelationIdContext y)
        {
            // Check whether the compared objects reference the same data
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            // Check whether the objects' properties are equal.
            return x.CorrelationId == y.CorrelationId;
        }

        public int GetHashCode([DisallowNull] CorrelationIdContext obj)
        {
            // Check whether the object is null
            if (ReferenceEquals(obj, null)) return 0;

            // Calculate the hash code for the object.
            return obj.CorrelationId.GetHashCode(StringComparison.InvariantCulture);
        }
    }
}
