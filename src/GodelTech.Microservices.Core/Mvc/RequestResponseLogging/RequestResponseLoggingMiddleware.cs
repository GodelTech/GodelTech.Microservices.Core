using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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

        private const string AuthorizationHeader = "Authorization";
        private static readonly HashSet<string> RequestHeadersToSkip = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            AuthorizationHeader,
            "Cookie"
        };

        /// <summary>
        /// Creates a new instance of the RequestResponseLoggingMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
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
            // code dealing with the request
            await using var requestBodyStream = _recyclableMemoryStreamManager.GetStream();
            context.Request.EnableBuffering();

            await context.Request.Body.CopyToAsync(requestBodyStream);


            var requestUriOriginalString = context.Request.GetEncodedUrl();
            var requestRemoteIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var requestHttpMethod = context.Request.Method;
            var requestAuthHeader = GetAndParseRequestAuthHeader(context);
            var requestAuthHeaderValue = GetRequestAuthHeaderStringValue(requestAuthHeader);
            var requestHeaders = GetHeadersDctionary(context.Request.Headers);




            await _next(context);

            // code dealing with the response
        }
        private static AuthenticationHeaderValue GetAndParseRequestAuthHeader(HttpContext context)
        {
            var header = context.Request.Headers[context.Request.Headers[AuthorizationHeader]];

            return string.IsNullOrEmpty(header) 
                ? null
                : AuthenticationHeaderValue.Parse(header);
        }

        private static string GetRequestAuthHeaderStringValue(AuthenticationHeaderValue authHeader)
        {
            return authHeader == null
                ? string.Empty 
                : AuthorizationHeader + "=" + authHeader.Scheme;
        }

        private IDictionary<string, string> GetHeadersDctionary(IHeaderDictionary headers)
        {
            try
            {
                return headers
                    .Where(x => !RequestHeadersToSkip.Contains(x.Key))
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.ToString()
                    );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Header dictionary creation failure");
            }

            return EmptyDictionary;
        }
    }
}