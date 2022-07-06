using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class RequestResponseLoggingMiddlewareInitializerTests
    {
        private readonly FakeRequestResponseLoggingMiddlewareInitializer _initializer;

        public RequestResponseLoggingMiddlewareInitializerTests()
        {
            _initializer = new FakeRequestResponseLoggingMiddlewareInitializer();
        }

        [Fact]
        public void ConfigureRequestResponseLoggingOptions_Success()
        {
            // Arrange
            var mockOptions = new Mock<RequestResponseLoggingOptions>(MockBehavior.Strict);

            // Act
            _initializer.ExposedConfigureRequestResponseLoggingOptions(mockOptions.Object);

            // Assert
            Assert.NotNull(mockOptions.Object);

            mockOptions.VerifyAll();
        }
    }
}
