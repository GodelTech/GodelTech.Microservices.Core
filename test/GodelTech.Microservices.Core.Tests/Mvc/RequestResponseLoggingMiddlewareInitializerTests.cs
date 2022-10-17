using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public void ConfigureEndpoints_Success()
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            // Act
            _initializer.ConfigureEndpoints(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            Assert.NotNull(_initializer);
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
