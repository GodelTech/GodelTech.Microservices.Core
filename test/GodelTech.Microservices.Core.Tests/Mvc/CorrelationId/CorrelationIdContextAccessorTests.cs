using GodelTech.Microservices.Core.Mvc.CorrelationId;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.CorrelationId
{
    public class CorrelationIdContextAccessorTests
    {
        private readonly CorrelationIdContextAccessor _accessor;

        public CorrelationIdContextAccessorTests()
        {
            _accessor = new CorrelationIdContextAccessor();
        }

        [Fact]
        public void CorrelationIdContext_Get_IsNull()
        {
            // Arrange & Act & Assert
            Assert.Null(_accessor.CorrelationIdContext);
        }

        [Fact]
        public void CorrelationIdContext_Set_Success()
        {
            // Arrange
            var context = new CorrelationIdContext("TestCorrelationId");

            // Act
            _accessor.CorrelationIdContext = context;

            // Assert
            Assert.Equal(context, _accessor.CorrelationIdContext);
        }

        [Fact]
        public void CorrelationIdContext_SetNull_ReturnsNull()
        {
            // Arrange
            var context = new CorrelationIdContext("TestCorrelationId");

            _accessor.CorrelationIdContext = context;

            // Act
            _accessor.CorrelationIdContext = null;

            // Assert
            Assert.Null(_accessor.CorrelationIdContext);
        }
    }
}