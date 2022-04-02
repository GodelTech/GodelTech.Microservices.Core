using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Mvc.LogUncaughtErrors
{
    /// <summary>
    /// Middleware which logs uncaught errors.
    /// </summary>
    public class LogUncaughtErrorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogUncaughtErrorsMiddleware> _logger;

        /// <summary>
        /// Creates a new instance of the CorrelationIdMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        public LogUncaughtErrorsMiddleware(
            RequestDelegate next,
            ILogger<LogUncaughtErrorsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private static readonly Action<ILogger, string, string, string, string, Exception> LogErrorCallback
            = LoggerMessage.Define<string, string, string, string>(
                LogLevel.Error,
                new EventId(0, nameof(InvokeAsync)),
                "Action={Action}," +
                "Message=Uncaught error:{ErrorMessage}," +
                "Method={RequestMethod}," +
                "RequestUri={RequestUrl}"
            );

        private async Task InvokeInternalAsync(HttpContext context)
        {
            // catch any errors that can propagate out of any middleware we run.
            // this will log the error, otherwise the error will be lost.
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    LogErrorCallback(
                        _logger,
                        "LogUncaughtErrors",
                        e.Message,
                        context.Request.Method,
                        context.Request.GetDisplayUrl(),
                        e
                    );
                }

                throw;
            }
        }
    }
}