using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

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

        /// <summary>
        /// Creates a new instance of the CorrelationIdMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="correlationIdContextFactory">The CorrelationIdContext factory.</param>
        public CorrelationIdMiddleware(
            RequestDelegate next,
            IOptions<CorrelationIdOptions> options,
            ICorrelationIdContextFactory correlationIdContextFactory)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _next = next;
            _options = options.Value;
            _correlationIdContextFactory = correlationIdContextFactory;
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var correlationId = GetCorrelationId(context);

            _correlationIdContextFactory.Create(correlationId);

            // apply the correlation ID to the response header for client side tracking
            context.Response.OnStarting(
                () =>
                {
                    if (!context.Response.Headers.ContainsKey(_options.ResponseHeader))
                    {
                        context.Response.Headers.Add(
                            _options.ResponseHeader,
                            correlationId
                        );
                    }

                    return Task.CompletedTask;
                }
            );

            await _next.Invoke(context);

            _correlationIdContextFactory.Dispose();
        }

        private string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(
                _options.RequestHeader,
                out var correlationIdValue
            );

            if (StringValues.IsNullOrEmpty(correlationIdValue))
            {
                return Guid.NewGuid().ToString();
            }

            return correlationIdValue.FirstOrDefault();
        }
    }
}