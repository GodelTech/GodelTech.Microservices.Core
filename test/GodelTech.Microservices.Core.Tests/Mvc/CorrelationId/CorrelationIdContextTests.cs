using GodelTech.Microservices.Core.Mvc.CorrelationId;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.CorrelationId
{
    public class CorrelationIdContextTests
    {
        [Fact]
        public void CorrelationId_Get_Success()
        {
            // Arrange
            const string expectedCorrelationId = "TestCorrelationId";

            var context = new CorrelationIdContext(expectedCorrelationId);

            // Act & Assert
            Assert.Equal(expectedCorrelationId, context.CorrelationId);
        }
    }
}
