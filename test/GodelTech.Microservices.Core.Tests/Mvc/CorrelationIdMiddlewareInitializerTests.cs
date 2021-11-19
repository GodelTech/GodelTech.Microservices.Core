using GodelTech.Microservices.Core.Mvc.CorrelationId;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
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
        public void ConfigureRazorPagesOptions_Success()
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