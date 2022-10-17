using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class LogUncaughtErrorsMiddlewareInitializerTests
    {
        private readonly LogUncaughtErrorsMiddlewareInitializer _initializer;

        public LogUncaughtErrorsMiddlewareInitializerTests()
        {
            _initializer = new LogUncaughtErrorsMiddlewareInitializer();
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
    }
}
