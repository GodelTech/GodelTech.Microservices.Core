using System.IO;
using System.Text;
using GodelTech.Microservices.Core.Mvc.RequestResponseLogging;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.RequestResponseLogging
{
    public class StreamExtensionsTests
    {
        [Fact]
        public void ReadInChunks_Success()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Test String"));

            // Act
            var result = stream.ReadInChunks();

            // Assert
            Assert.Equal("Test String", result);
        }
    }
}