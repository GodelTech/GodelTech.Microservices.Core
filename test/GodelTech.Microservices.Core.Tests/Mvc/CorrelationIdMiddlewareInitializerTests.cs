using GodelTech.Microservices.Core.Mvc.CorrelationId;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class CorrelationIdMiddlewareInitializerTests
    {
        private readonly FakeCorrelationIdMiddlewareInitializer _initializer;

        public CorrelationIdMiddlewareInitializerTests()
        {
            _initializer = new FakeCorrelationIdMiddlewareInitializer();
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
        public void ConfigureCorrelationIdOptions_Success()
        {
            // Arrange
            var mockOptions = new Mock<CorrelationIdOptions>(MockBehavior.Strict);

            // Act
            _initializer.ExposedConfigureCorrelationIdOptions(mockOptions.Object);

            // Assert
            Assert.NotNull(mockOptions.Object);

            mockOptions.VerifyAll();
        }
    }
}
