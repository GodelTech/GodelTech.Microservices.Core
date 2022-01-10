using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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
            RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _next = next;
            _options = options.Value;
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

            await LogRequest(context);

            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            var body = string.Empty;

            if (_options.IncludeRequestBody)
            {
                context.Request.EnableBuffering();

                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);

                body = requestStream.ReadInChunks();

                context.Request.Body.Position = 0;
            }

            _logger.LogInformation(
                $"Http Request Information:{Environment.NewLine}" +
                $"TraceIdentifier: {context.TraceIdentifier}," +
                $"Method: {context.Request.Method}," +
                $"Url: {context.Request.GetEncodedUrl()}," +
                $"RemoteIP: {context.Request.HttpContext.Connection.RemoteIpAddress}," +
                $"RequestHeaders: {JsonSerializer.Serialize(context.Request.Headers)}" +
                (_options.IncludeRequestBody ? $",Body: {body}" : string.Empty)
            );
        }

        private async Task LogResponse(HttpContext context)
        {
            var timer = Stopwatch.StartNew();

            var body = string.Empty;

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

            _logger.LogInformation(
                $"Http Response Information:{Environment.NewLine}" +
                $"TraceIdentifier: {context.TraceIdentifier}," +
                $"StatusCode: {context.Response.StatusCode}," +
                $"ReasonPhrase: {context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase}," +
                $"ResponseTimeMilliseconds: {timer.ElapsedMilliseconds}," +
                $"ResponseHeaders: {JsonSerializer.Serialize(context.Response.Headers)}" +
                (_options.IncludeResponseBody ? $",Body: {body}" : string.Empty)
            );
        }
    }
}