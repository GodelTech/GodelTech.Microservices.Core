﻿using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.RequestResponseLogging
{
    public class RequestResponseLoggingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockRequestDelegate;
        private readonly RequestResponseLoggingOptions _options;
        private readonly Mock<ILogger<RequestResponseLoggingMiddleware>> _mockLogger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        private readonly RequestResponseLoggingMiddleware _middleware;

        public RequestResponseLoggingMiddlewareTests()
        {
            _mockRequestDelegate = new Mock<RequestDelegate>(MockBehavior.Strict);

            _options = new RequestResponseLoggingOptions();

            var mockOptions = new Mock<IOptions<RequestResponseLoggingOptions>>(MockBehavior.Strict);
            mockOptions
                .Setup(x => x.Value)
                .Returns(_options);

            _mockLogger = new Mock<ILogger<RequestResponseLoggingMiddleware>>(MockBehavior.Strict);
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

            _middleware = new RequestResponseLoggingMiddleware(
                _mockRequestDelegate.Object,
                mockOptions.Object,
                _mockLogger.Object,
                _recyclableMemoryStreamManager
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new RequestResponseLoggingMiddleware(
                    _mockRequestDelegate.Object,
                    null,
                    _mockLogger.Object,
                    _recyclableMemoryStreamManager
                )
            );

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _middleware.InvokeAsync(null)
            );

            Assert.Equal("context", exception.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_WhenLoggerIsNotEnabled()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(false);

            _mockRequestDelegate
                .Setup(x => x.Invoke(httpContext))
                .Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(httpContext);

            // Assert
            _mockRequestDelegate
                .Verify(
                    x => x.Invoke(httpContext),
                    Times.Once
                );
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task InvokeAsync_Success(
            bool includeRequestBody,
            bool includeResponseBody)
        {
            // Arrange
            _options.IncludeRequestBody = includeRequestBody;
            _options.IncludeResponseBody = includeResponseBody;

            var httpContext = new DefaultHttpContext
            {
                TraceIdentifier = "Test TraceIdentifier",
                Request =
                {
                    Method = "POST",
                    Scheme = "http",
                    Host = new HostString("localhost", 80),
                    Path = "/fakes",
                    QueryString = new QueryString("?version=1"),
                    HttpContext =
                    {
                        Connection =
                        {
                            RemoteIpAddress = IPAddress.Parse("172.0.0.1")
                        }
                    },
                    Headers =
                    {
                        { "Test RequestHeader Key", "Test RequestHeader Value" }
                    },
                    Body = new MemoryStream(Encoding.UTF8.GetBytes("Test RequestBody"))
                },
                Response =
                {
                    StatusCode = 201,
                    Headers =
                    {
                        { "Test ResponseHeader Key", "Test ResponseHeader Value" }
                    },
                    Body = new MemoryStream()
                }
            };
            httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Test ReasonPhrase";
            httpContext.Response.Body.Write(Encoding.UTF8.GetBytes("Test ResponseBody"));

            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true);

            var requestHeaders = JsonSerializer.Serialize(httpContext.Request.Headers);
            Expression<Action<ILogger<RequestResponseLoggingMiddleware>>> loggerExpressionRequest = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Http Request Information:{Environment.NewLine}" +
                    $"TraceIdentifier: {httpContext.TraceIdentifier}," +
                    $"Method: {httpContext.Request.Method}," +
                    $"Url: {httpContext.Request.GetEncodedUrl()}," +
                    $"RemoteIP: {httpContext.Request.HttpContext.Connection.RemoteIpAddress}," +
                    $"RequestHeaders: {requestHeaders}" +
                    (includeRequestBody ? ",Body: Test RequestBody" : string.Empty)
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionRequest);

            _mockRequestDelegate
                .Setup(x => x.Invoke(httpContext))
                .Callback(
                    () =>
                    {
                        httpContext.Response.Body.Write(Encoding.UTF8.GetBytes("Test NewResponseBody"));
                    }
                )
                .Returns(Task.CompletedTask);

            Expression<Action<ILogger<RequestResponseLoggingMiddleware>>> loggerExpressionResponse = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    new Regex(
                        "^" +
                        $"Http Response Information:{Environment.NewLine}" +
                        $"TraceIdentifier: {httpContext.TraceIdentifier}," +
                        $"StatusCode: {httpContext.Response.StatusCode}," +
                        $"ReasonPhrase: {httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase}," +
                        "ResponseTimeMilliseconds: [0-9]{1,}," +
                        @"ResponseHeaders: {""Test ResponseHeader Key"":\[""Test ResponseHeader Value""\]}" +
                        (includeResponseBody ? ",Body: Test NewResponseBody" : string.Empty) +
                        "$"
                    ).IsMatch(v.ToString())
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionResponse);

            // Act
            await _middleware.InvokeAsync(httpContext);

            // Assert
            _mockLogger.Verify(loggerExpressionRequest, Times.Once);

            _mockRequestDelegate
                .Verify(
                    x => x.Invoke(httpContext),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionResponse, Times.Once);

            if (includeRequestBody)
            {
                Assert.Equal(0, httpContext.Request.Body.Position);
            }
        }
    }
}