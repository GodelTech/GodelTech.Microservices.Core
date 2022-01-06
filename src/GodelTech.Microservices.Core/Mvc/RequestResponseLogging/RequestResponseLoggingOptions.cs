namespace GodelTech.Microservices.Core.Mvc.RequestResponseLogging
{
    /// <summary>
    /// Options for RequestResponseLogging.
    /// </summary>
    public class RequestResponseLoggingOptions
    {
        /// <summary>
        /// Includes request body in log.
        /// </summary>
        public bool IncludeRequestBody { get; set; } = true;

        /// <summary>
        /// Includes response body in log.
        /// </summary>
        public bool IncludeResponseBody { get; set; } = true;
    }
}