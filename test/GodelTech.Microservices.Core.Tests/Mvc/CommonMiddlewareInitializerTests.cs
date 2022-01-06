using System.Linq;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class CommonMiddlewareInitializerTests
    {
        private readonly FakeCommonMiddlewareInitializer _initializer;

        public CommonMiddlewareInitializerTests()
        {
            _initializer = new FakeCommonMiddlewareInitializer();
        }

        [Fact]
        public void CreateInitializers_Success()
        {
            // Arrange & Act
            var result = _initializer
                .ExposedCreateInitializers()
                .ToList();

            // Assert
            Assert.Equal(3, result.Count);

            Assert.IsType<CorrelationIdMiddlewareInitializer>(result[0]);
            Assert.IsType<RequestResponseLoggingMiddlewareInitializer>(result[1]);
            Assert.IsType<LogUncaughtErrorsMiddlewareInitializer>(result[2]);
        }
    }
}