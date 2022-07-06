using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.RequestResponseLogging
{
    public class RequestResponseLoggingOptionsTests
    {
        private readonly RequestResponseLoggingOptions _options;

        public RequestResponseLoggingOptionsTests()
        {
            _options = new RequestResponseLoggingOptions();
        }

        [Fact]
        public void IncludeRequestBody_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.IncludeRequestBody);
        }

        [Fact]
        public void IncludeRequestBody_Set_Success()
        {
            // Arrange
            const bool expectedResult = false;

            // Act
            _options.IncludeRequestBody = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.IncludeRequestBody);
        }

        [Fact]
        public void IncludeResponseBody_Get_Success()
        {
            // Arrange & Act & Assert
            Assert.True(_options.IncludeResponseBody);
        }

        [Fact]
        public void IncludeResponseBody_Set_Success()
        {
            // Arrange
            const bool expectedResult = false;

            // Act
            _options.IncludeResponseBody = expectedResult;

            // Assert
            Assert.Equal(expectedResult, _options.IncludeResponseBody);
        }
    }
}
