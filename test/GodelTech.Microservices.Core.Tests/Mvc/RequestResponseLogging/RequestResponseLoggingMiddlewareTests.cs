using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using GodelTech.Microservices.Core.Utilities;
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
        private readonly Mock<IStopwatch> _mockStopwatch;

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
            _mockStopwatch = new Mock<IStopwatch>(MockBehavior.Strict);

            var mockStopwatchFactory = new Mock<IStopwatchFactory>(MockBehavior.Strict);
            mockStopwatchFactory
                .Setup(x => x.Create())
                .Returns(_mockStopwatch.Object);

            _middleware = new RequestResponseLoggingMiddleware(
                _mockRequestDelegate.Object,
                mockOptions.Object,
                _mockLogger.Object,
                _recyclableMemoryStreamManager,
                mockStopwatchFactory.Object
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

            _mockStopwatch
                .Setup(x => x.Start());

            _mockStopwatch
                .Setup(x => x.Stop());

            _mockStopwatch
                .Setup(x => x.ElapsedMilliseconds)
                .Returns(12345);

            var requestHeaders = JsonSerializer.Serialize(httpContext.Request.Headers);
            Expression<Action<ILogger<RequestResponseLoggingMiddleware>>> loggerExpressionRequest = x => x.Log(
                LogLevel.Information,
                new EventId(0, "LogRequest"),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Http Request Information:" + Environment.NewLine +
                        "TraceIdentifier: {0}," +
                        "Method: {1}," +
                        "Url: {2}," +
                        "RemoteIP: {3}," +
                        "RequestHeaders: {4}," +
                        "Body: {5}",
                        httpContext.TraceIdentifier,
                        httpContext.Request.Method,
                        httpContext.Request.GetEncodedUrl(),
                        httpContext.Request.HttpContext.Connection.RemoteIpAddress,
                        requestHeaders,
                        _options.IncludeRequestBody
                            ? "Test RequestBody"
                            : "<IncludeRequestBody is false>"
                    )
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
                new EventId(0, "LogResponse"),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Http Response Information:" + Environment.NewLine +
                        "TraceIdentifier: {0}," +
                        "StatusCode: {1}," +
                        "ReasonPhrase: {2}," +
                        "ResponseTimeMilliseconds: {3}," +
                        "ResponseHeaders: {4}," +
                        "Body: {5}",
                        httpContext.TraceIdentifier,
                        httpContext.Response.StatusCode,
                        httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase,
                        12345,
                        @"{""Test ResponseHeader Key"":[""Test ResponseHeader Value""]}",
                        _options.IncludeResponseBody
                            ? "Test NewResponseBody"
                            : "<IncludeResponseBody is false>"
                    )
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

            _mockStopwatch
                .Verify(
                    x => x.Start(),
                    Times.Once
                );

            _mockStopwatch
                .Verify(
                    x => x.Stop(),
                    Times.Once
                );

            _mockStopwatch
                .Verify(
                    x => x.ElapsedMilliseconds,
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionResponse, Times.Once);
        }
    }
}
