namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// Options for CorrelationId.
    /// </summary>
    public class CorrelationIdOptions
    {
        private string _responseHeader;

        /// <summary>
        /// The name of the header from which the CorrelationId is read from the request.
        /// </summary>
        public string RequestHeader { get; set; } = "X-Correlation-ID";

        /// <summary>
        /// The name of the header to which the Correlation ID is written for the response.
        /// </summary>
        public string ResponseHeader
        {
            get => _responseHeader ?? RequestHeader;
            set => _responseHeader = value;
        }
    }
}
