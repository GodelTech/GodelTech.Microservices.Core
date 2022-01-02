using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.LogUncaughtErrors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.LogUncaughtErrors
{
    public class LogUncaughtErrorsMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockRequestDelegate;
        private readonly Mock<ILogger<LogUncaughtErrorsMiddleware>> _mockLogger;

        private readonly LogUncaughtErrorsMiddleware _middleware;

        public LogUncaughtErrorsMiddlewareTests()
        {
            _mockRequestDelegate = new Mock<RequestDelegate>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<LogUncaughtErrorsMiddleware>>(MockBehavior.Strict);

            _middleware = new LogUncaughtErrorsMiddleware(
                _mockRequestDelegate.Object,
                _mockLogger.Object
            );
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
        public async Task InvokeAsync_Success()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

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

        [Fact]
        public async Task InvokeAsync_LogErrorAndRethrowException()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            var expectedException = new InvalidOperationException("Test exception message.");

            _mockRequestDelegate
                .Setup(x => x.Invoke(httpContext))
                .ThrowsAsync(expectedException);

            Expression<Action<ILogger<LogUncaughtErrorsMiddleware>>> loggerExpression = x => x.Log(
                LogLevel.Error,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    "Action=LogUncaughtErrors, Message=Uncaught error:Test exception message., Method=, RequestUri=://"
                ),
                It.Is<InvalidOperationException>(e => e == expectedException),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpression);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _middleware.InvokeAsync(httpContext)
            );

            Assert.Equal(expectedException, exception);

            _mockRequestDelegate
                .Verify(
                    x => x.Invoke(httpContext),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpression, Times.Once);
        }
    }
}