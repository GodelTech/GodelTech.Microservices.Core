namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// A factory for creating <see cref="CorrelationIdContext" /> instances.
    /// </summary>
    public class CorrelationIdContextFactory : ICorrelationIdContextFactory
    {
        private readonly ICorrelationIdContextAccessor _correlationIdContextAccessor;

        /// <summary>
        /// Creates a factory for creating <see cref="CorrelationIdContext" /> instances.
        /// </summary>
        /// <param name="correlationIdContextAccessor">The <see cref="ICorrelationIdContextAccessor"/>.</param>
        public CorrelationIdContextFactory(ICorrelationIdContextAccessor correlationIdContextAccessor)
        {
            _correlationIdContextAccessor = correlationIdContextAccessor;
        }

        /// <inheritdoc/>
        public CorrelationIdContext Create(string correlationId)
        {
            var correlationIdContext = new CorrelationIdContext(correlationId);

            if (_correlationIdContextAccessor != null)
            {
                _correlationIdContextAccessor.CorrelationIdContext = correlationIdContext;
            }

            return correlationIdContext;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_correlationIdContextAccessor != null)
            {
                _correlationIdContextAccessor.CorrelationIdContext = null;
            }
        }
    }
}