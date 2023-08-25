using System;

namespace GodelTech.Microservices.Core.Utilities
{
    /// <summary>
    /// Guid.
    /// </summary>
    public interface IGuid
    {
        /// <summary>
        /// Creates new Guid.
        /// </summary>
        /// <returns>New Guid.</returns>
        Guid NewGuid();
    }
}
