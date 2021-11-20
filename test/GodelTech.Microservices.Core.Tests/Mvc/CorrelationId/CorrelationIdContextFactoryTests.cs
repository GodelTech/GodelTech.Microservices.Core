using GodelTech.Microservices.Core.Mvc.CorrelationId;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc.CorrelationId;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.CorrelationId
{
    public class CorrelationIdContextFactoryTests
    {
        private readonly Mock<ICorrelationIdContextAccessor> _mockCorrelationIdContextAccessor;

        private readonly CorrelationIdContextFactory _factory;

        public CorrelationIdContextFactoryTests()
        {
            _mockCorrelationIdContextAccessor = new Mock<ICorrelationIdContextAccessor>(MockBehavior.Strict);

            _factory = new CorrelationIdContextFactory(_mockCorrelationIdContextAccessor.Object);
        }

        [Fact]
        public void Create_WhenAccessorIsNull_ReturnsContext()
        {
            // Arrange
            const string correlationId = "TestCorrelationId";

            var expectedResult = new CorrelationIdContext(correlationId);

            var factory = new CorrelationIdContextFactory(null);

            // Act
            var result = factory.Create(correlationId);

            // Assert
            Assert.Equal(expectedResult, result, new CorrelationIdContextEqualityComparer());
        }

        [Fact]
        public void Create_ReturnsContext()
        {
            // Arrange
            const string correlationId = "TestCorrelationId";

            var expectedResult = new CorrelationIdContext(correlationId);

            _mockCorrelationIdContextAccessor
                .SetupSet(
                    x =>
                        x.CorrelationIdContext = It.Is(
                            expectedResult,
                            new CorrelationIdContextEqualityComparer()
                        )
                );

            // Act
            var result = _factory.Create(correlationId);

            // Assert
            _mockCorrelationIdContextAccessor
                .VerifySet(
                    x =>
                        x.CorrelationIdContext = It.Is(
                            expectedResult,
                            new CorrelationIdContextEqualityComparer()
                        ),
                    Times.Once
                );

            Assert.Equal(expectedResult, result, new CorrelationIdContextEqualityComparer());
        }

        [Fact]
        public void Clear_WhenAccessorIsNull()
        {
            // Arrange
            var context = new CorrelationIdContext("TestCorrelationId");

            var factory = new CorrelationIdContextFactory(null);

            // Act & Assert
            factory.Clear(context);
        }

        [Fact]
        public void Clear_Success()
        {
            // Arrange
            var context = new CorrelationIdContext("TestCorrelationId");

            _mockCorrelationIdContextAccessor
                .SetupSet(x => x.CorrelationIdContext = null);

            // Act
            _factory.Clear(context);

            // Assert
            _mockCorrelationIdContextAccessor
                .VerifySet(
                    x => x.CorrelationIdContext = null,
                    Times.Once
                );
        }
    }
}