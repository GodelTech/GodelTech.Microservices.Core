using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace GodelTech.Microservices.Core.Mvc.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        public const string BearerAuthScheme = "Bearer";
        private const string AuthorizationHeader = "Authorization";

        private static readonly Dictionary<string, string> EmptyDictionary = new Dictionary<string, string>();

        private static readonly HashSet<string> RequestHeadersToSkip = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            AuthorizationHeader,
            "Cookie"
        };

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip this middleware if Info log level is not enabled
            if (!_logger.IsEnabled(LogLevel.Information))
            {
                await _next.Invoke(context);
                return;
            }

            // Process request and log response

            var requestUriOriginalString = string.Empty;
            var requestRemoteIpAddress = string.Empty;
            var requestHttpMethod = string.Empty;
            AuthenticationHeaderValue requestAuthHeader = null;
            var requestAuthHeaderStr = string.Empty;
            IDictionary<string, string> requestHeaders = EmptyDictionary;

            var timer = Stopwatch.StartNew();

            try
            {

                // Read the request properties before processing. 
                // This will prevent getting
                // {"Cannot access a disposed object.\r\nObject name: 'System.Net.HttpListenerRequest'."}
                // See https://tickets.intelliflo.com/browse/IP-29122
                requestUriOriginalString = GetUri(context.Request)?.OriginalString;
                requestRemoteIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                requestHttpMethod = context.Request.Method;
                requestAuthHeader = GetAndParseRequestAuthHeader(context);
                requestAuthHeaderStr = AuthHeaderToString(requestAuthHeader);
                requestHeaders = ToDictionary(context.Request.Headers);

                await _next.Invoke(context);
            }
            finally
            {
                timer.Stop();
                try
                {
                    _logger.LogInformation(
                        "Method={method}," +
                        "Uri={uri}," +
                        "RemoteIP={remoteIP}, " +
                        "ResponseTimeMs={responseTimeMs}, " +
                        "StatusCode={statusCode}, " +
                        "ReasonPhrase={reasonPhrase}, " +
                        "RequestHeaders={@requestHeaders}, " +
                        "RequestAuthHeader={requestAuthHeader}, " +
                        "ResponseHeaders={@responseHeaders}, " +
                        "AuthClaims={@authClaims}",

                        requestHttpMethod,
                        requestUriOriginalString,
                        requestRemoteIpAddress,
                        timer.ElapsedMilliseconds,
                        context.Response.StatusCode,
                        context.Response.HttpContext.Features.Get<IHttpResponseFeature>()?.ReasonPhrase,
                        requestHeaders, 
                        requestAuthHeaderStr, 
                        ToDictionary(context.Response.Headers), 
                        GetAuthClaims(requestAuthHeader)
                        );
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to log request details");
                }
            }
        }

        private IDictionary<string, string> ToDictionary(IHeaderDictionary headers)
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
                _logger.LogError(e, "Header message creation failure");
            }

            return EmptyDictionary;
        }

        private static IDictionary<string, string> GetAuthClaims(AuthenticationHeaderValue header)
        {
            if (header == null)
                return EmptyDictionary;

            return header.Scheme == BearerAuthScheme ? 
                GetJwtClaims(header.Parameter) :
                EmptyDictionary;
        }

        private static IDictionary<string, string> GetJwtClaims(string headerParameter)
        {
            if (string.IsNullOrWhiteSpace(headerParameter))
                return EmptyDictionary;

            var jwtHandler = new JwtSecurityTokenHandler();
            var readableToken = jwtHandler.CanReadToken(headerParameter);
            if (readableToken == false)
                return EmptyDictionary;

            JwtSecurityToken token;
            try
            {
                token = jwtHandler.ReadToken(headerParameter) as JwtSecurityToken;
            }
            catch (Exception)
            {
                return EmptyDictionary;
            }

            return token == null ?
                EmptyDictionary : 
                BuildClaimsLog(token.Claims.ToArray());
        }

        private static IDictionary<string, string> BuildClaimsLog(Claim[] claims)
        {
            if (!claims.Any())
                return EmptyDictionary;

            return
                claims
                    .GroupBy(y => y.Type, v => v.Value)
                    .ToDictionary(x => x.Key, x => string.Join(", ", x));
        }

        private static AuthenticationHeaderValue GetAndParseRequestAuthHeader(HttpContext context)
        {
            var header = context.Request.Headers[AuthorizationHeader];

            return string.IsNullOrEmpty(header) ? 
                null : 
                AuthenticationHeaderValue.Parse(header);
        }

        private static string AuthHeaderToString(AuthenticationHeaderValue authHeader)
        {
            return authHeader == null ? 
                string.Empty : 
                AuthorizationHeader + "=" + authHeader.Scheme;
        }

        private static Uri GetUri(HttpRequest request)
        {
            var builder = new StringBuilder();

            builder.Append(request.Scheme).Append("://").Append(request.Host);

            if (request.Path.HasValue)
                builder.Append(request.Path.Value);

            if (request.QueryString.HasValue)
                builder.Append(request.QueryString);

            return new Uri(builder.ToString());
        }
    }
}
