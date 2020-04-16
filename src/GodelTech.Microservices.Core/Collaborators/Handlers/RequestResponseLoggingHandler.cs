using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Collaborators.Handlers
{
    /// <summary>
    /// IMPORTANT: This component should be only dependent on SingleInstance() components.
    /// Transient instances may cause difficulties with tracing side effects raised by
    /// their state or other transient deps (for example, per-request auth).
    /// https://github.com/aspnet/HttpClientFactory/issues/198
    /// https://github.com/aspnet/Docs/issues/9306
    /// </summary>
    public class RequestResponseLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<RequestResponseLoggingHandler> _logger;

        public RequestResponseLoggingHandler(ILogger<RequestResponseLoggingHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Action={action}, Method={method}, Uri={uri}",
                "HttpRequestSending",
                request.Method,
                request.RequestUri);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                _logger.LogInformation(
                    "Action={action}, Method={method}, Uri={uri}, CallDurationMs={callDurationMs}, ResponseStatusCode={responseStatusCode}, ReasonPhrase='{reasonPhrase}'",
                    "HttpRequestSent",
                    request.Method.Method,
                    request.RequestUri,
                    stopwatch.ElapsedMilliseconds,
                    response.StatusCode,
                    response.ReasonPhrase
                );

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Action={action}, Method={method}, Uri={uri}, CallDurationMs={callDurationMs}",
                    "HttpRequestError",
                    request.Method.Method,
                    request.RequestUri,
                    stopwatch.ElapsedMilliseconds
                );

                throw;
            }
        }
    }
}
