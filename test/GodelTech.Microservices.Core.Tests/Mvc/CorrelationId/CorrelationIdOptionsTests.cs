using GodelTech.Microservices.Core.Mvc.CorrelationId;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.CorrelationId
{
    public class CorrelationIdOptionsTests
    {
        private readonly CorrelationIdOptions _options;

        public CorrelationIdOptionsTests()
        {
            _options = new CorrelationIdOptions();
        }

        [Fact]
        public void RequestHeader_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Equal("X-Correlation-ID", _options.RequestHeader);
        }

        [Fact]
        public void RequestHeader_Set_Success()
        {
            // Arrange
            var expectedResult = "test-request-header";

            // Act
            _options.RequestHeader = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.RequestHeader);
            Assert.Equal(expectedResult, _options.ResponseHeader);
        }

        [Fact]
        public void ResponseHeader_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.Equal("X-Correlation-ID", _options.ResponseHeader);
        }

        [Fact]
        public void ResponseHeader_Set_Success()
        {
            // Arrange
            var expectedResult = "test-response-header";

            // Act
            _options.ResponseHeader = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.ResponseHeader);
        }
    }
}