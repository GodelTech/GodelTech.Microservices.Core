namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// Provides methods to create and dispose of <see cref="CorrelationIdContext"/> instances.
    /// </summary>
    public interface ICorrelationIdContextFactory
    {
        /// <summary>
        /// Creates an <see cref="CorrelationIdContext"/> instance.
        /// </summary>
        /// <param name="correlationId">The correlation Id.</param>
        /// <returns>The <see cref="CorrelationIdContext"/> instance.</returns>
        CorrelationIdContext Create(string correlationId);

        /// <summary>
        /// Releases resources held by the <see cref="CorrelationIdContext"/>.
        /// </summary>
        /// <param name="correlationIdContext">The <see cref="CorrelationIdContext"/> to dispose.</param>
        void Clear(CorrelationIdContext correlationIdContext);
    }
}
