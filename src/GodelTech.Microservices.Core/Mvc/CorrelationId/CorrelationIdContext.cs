namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// Encapsulates all CorrelationId specific information.
    /// </summary>
    public class CorrelationIdContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdContext"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation Id.</param>
        public CorrelationIdContext(string correlationId)
        {
            CorrelationId = correlationId;
        }

        /// <summary>
        /// The CorrelationId.
        /// </summary>
        public string CorrelationId { get; }
    }
}
