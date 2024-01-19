using System;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// Middleware which attempts to reads / creates a Correlation Id that can then be used in logs and passed to upstream requests.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdOptions _options;
        private readonly ICorrelationIdContextFactory _correlationIdContextFactory;
        private readonly IGuid _guid;

        /// <summary>
        /// Creates a new instance of the CorrelationIdMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="correlationIdContextFactory">The CorrelationIdContext factory.</param>
        /// <param name="guid">The Guid utility.</param>
        public CorrelationIdMiddleware(
            RequestDelegate next,
            IOptions<CorrelationIdOptions> options,
            ICorrelationIdContextFactory correlationIdContextFactory,
#pragma warning disable CA1720
            IGuid guid = default(SystemGuid))
#pragma warning restore CA1720
        {
            ArgumentNullException.ThrowIfNull(options);

            _next = next;
            _options = options.Value;
            _correlationIdContextFactory = correlationIdContextFactory;
            _guid = guid ?? new SystemGuid();
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        public Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return InvokeInternalAsync(context);
        }

        private async Task InvokeInternalAsync(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);

            var correlationIdContext = _correlationIdContextFactory.Create(correlationId);

            // apply the correlation ID to the response header for client side tracking
            context.Response.OnStarting(
                () =>
                {
                    if (!context.Response.Headers.ContainsKey(_options.ResponseHeader))
                    {
                        context.Response.Headers.Append(
                            _options.ResponseHeader,
                            correlationId
                        );
                    }

                    return Task.CompletedTask;
                }
            );

            await _next.Invoke(context);

            _correlationIdContextFactory.Clear(correlationIdContext);
        }

        private string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(
                _options.RequestHeader,
                out var correlationIdValue
            );

            var correlationId = correlationIdValue.FirstOrDefault();

            return string.IsNullOrWhiteSpace(correlationId)
                ? _guid.NewGuid().ToString()
                : correlationId;
        }
    }
}
