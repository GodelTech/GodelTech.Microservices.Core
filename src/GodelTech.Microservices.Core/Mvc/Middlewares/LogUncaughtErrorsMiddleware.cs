using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Mvc.Middlewares
{
    public class LogUncaughtErrorsMiddleware
    {
        private const string LogUncaughtErrors = "LogUncaughtErrors";

        private readonly RequestDelegate _next;
        private readonly ILogger<LogUncaughtErrorsMiddleware> _logger;

        public LogUncaughtErrorsMiddleware(RequestDelegate next, ILogger<LogUncaughtErrorsMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // catch any errors that can propagate out of any middlewares we run.
            // this will log the error, otherwise the error will be lost.
            try
            {
                await _next(context);
            }
            catch (TaskCanceledException tex)
            {
                if (context.Request.Method.Equals("Head", StringComparison.OrdinalIgnoreCase))
                    _logger.LogInformation(
                        "Action={action}, Message=Uncaught error:{errorMessage} , Method={requestMethod}, RequestUri={requestUrl}",
                        LogUncaughtErrors,
                        tex.Message,
                        context.Request.Method,
                        context.Request.GetDisplayUrl());
                else
                {
                    _logger.LogWarning(
                        tex,
                        "Action={action}, Message=Uncaught error:{errorMessage} , Method={requestMethod}, RequestUri={requestUrl}",
                        LogUncaughtErrors,
                        tex.Message,
                        context.Request.Method,
                        context.Request.GetDisplayUrl()
                    );

                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    e,
                    "Action={action}, Message=Uncaught error:{errorMessage} , Method={requestMethod}, RequestUri={requestUrl}",
                    LogUncaughtErrors,
                    e.Message,
                    context.Request.Method,
                    context.Request.GetDisplayUrl()
                );

                throw;
            }
        }
    }
}
