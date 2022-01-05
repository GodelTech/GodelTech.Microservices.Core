using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace GodelTech.Microservices.Core.Mvc.RequestResponseLogging
{
    /// <summary>
    /// Middleware which logs requests and responses.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        /// <summary>
        /// Creates a new instance of the RequestResponseLoggingMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="recyclableMemoryStreamManager">The recyclableMemoryStreamManager.</param>
        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger, 
            RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        public Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return InvokeInternalAsync(context);
        }

        private async Task InvokeInternalAsync(HttpContext context)
        {
            if (!_logger.IsEnabled(LogLevel.Information))
            {
                await _next.Invoke(context);
                return;
            }

            LogRequest(context);

            await _next(context);

            LogResponse(context);
        }

        private void LogRequest(HttpContext context)
        {
            // todo: enable log request body
            _logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                                   $"Schema: {context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString}");
        }

        private void LogResponse(HttpContext context)
        {
            // todo: enable log response body
            _logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                                   $"Schema: {context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString}");
        }
    }
}