using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;

namespace GodelTech.Microservices.Core.Mvc.RequestResponseLogging
{
    /// <summary>
    /// Middleware which logs requests and responses.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestResponseLoggingOptions _options;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly IStopwatchFactory _stopwatchFactory;

        /// <summary>
        /// Creates a new instance of the RequestResponseLoggingMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="recyclableMemoryStreamManager">The recyclableMemoryStreamManager.</param>
        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            IOptions<RequestResponseLoggingOptions> options,
            ILogger<RequestResponseLoggingMiddleware> logger,
            RecyclableMemoryStreamManager recyclableMemoryStreamManager,
            IStopwatchFactory stopwatchFactory = default(SystemStopwatchFactory))
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _next = next;
            _options = options.Value;
            _logger = logger;
            _recyclableMemoryStreamManager = recyclableMemoryStreamManager;
            _stopwatchFactory = stopwatchFactory ?? new SystemStopwatchFactory();
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

            await LogRequestAsync(context);

            await LogResponseAsync(context);
        }

        private static readonly Action<ILogger, string, string, string, IPAddress, string, string, Exception> LogRequestCallback
            = LoggerMessage.Define<string, string, string, IPAddress, string, string>(
                LogLevel.Information,
                new EventId(0, nameof(LogRequestAsync)),
                "Http Request Information:" + Environment.NewLine +
                "TraceIdentifier: {TraceIdentifier}," +
                "Method: {Method}," +
                "Url: {Url}," +
                "RemoteIP: {RemoteIpAddress}," +
                "RequestHeaders: {RequestHeaders}," +
                "Body: {Body}"
            );

        private async Task LogRequestAsync(HttpContext context)
        {
            // Stryker disable once string
            var body = string.Empty;

            if (_options.IncludeRequestBody)
            {
                context.Request.EnableBuffering();

                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);

                body = requestStream.ReadInChunks();

                context.Request.Body.Position = 0;
            }

            LogRequestCallback(
                _logger,
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.GetEncodedUrl(),
                context.Request.HttpContext.Connection.RemoteIpAddress,
                JsonSerializer.Serialize(context.Request.Headers),
                _options.IncludeRequestBody
                    ? body
                    : "<IncludeRequestBody is false>",
                null
            );
        }

        private static readonly Action<ILogger, string, int, string, long, string, string, Exception> LogResponseCallback
            = LoggerMessage.Define<string, int, string, long, string, string>(
                LogLevel.Information,
                new EventId(0, nameof(LogResponseAsync)),
                "Http Response Information:" + Environment.NewLine +
                "TraceIdentifier: {TraceIdentifier}," +
                "StatusCode: {StatusCode}," +
                "ReasonPhrase: {ReasonPhrase}," +
                "ResponseTimeMilliseconds: {ResponseTimeMilliseconds}," +
                "ResponseHeaders: {ResponseHeaders}," +
                "Body: {Body}"
            );

        private async Task LogResponseAsync(HttpContext context)
        {
            // Stryker disable once string
            var body = string.Empty;

            var timer = _stopwatchFactory.Create();
            timer.Start();

            if (_options.IncludeResponseBody)
            {
                var originalBodyStream = context.Response.Body;

                await using var responseBody = _recyclableMemoryStreamManager.GetStream();
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                using var streamReader = new StreamReader(context.Response.Body);
                body = await streamReader.ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                await _next(context);
            }

            timer.Stop();

            LogResponseCallback(
                _logger,
                context.TraceIdentifier,
                context.Response.StatusCode,
                context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase,
                timer.ElapsedMilliseconds,
                JsonSerializer.Serialize(context.Response.Headers),
                _options.IncludeResponseBody
                    ? body
                    : "<IncludeResponseBody is false>",
                null
            );
        }
    }
}
