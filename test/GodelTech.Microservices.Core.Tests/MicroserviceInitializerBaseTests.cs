using System;
using GodelTech.Microservices.Core.Tests.Fakes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests
{
    public class MicroserviceInitializerBaseTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        private readonly FakeMicroserviceInitializerBase _initializer;

        public MicroserviceInitializerBaseTests()
        {
            _mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);

            _initializer = new FakeMicroserviceInitializerBase(_mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FakeMicroserviceInitializerBase(null)
            );

            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void Configuration_Success()
        {
            // Arrange & Act
            var result = _initializer.ExposedConfiguration;

            // Assert
            Assert.Equal(_mockConfiguration.Object, result);
        }

        [Fact]
        public void ConfigureServices_Success()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);

            // Act
            _initializer.ConfigureServices(mockServiceCollection.Object);

            // Assert
            _mockConfiguration.VerifyAll();

            mockServiceCollection.VerifyAll();
        }

        [Fact]
        public void Configure_Success()
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            // Act
            _initializer.Configure(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            _mockConfiguration.VerifyAll();

            mockApplicationBuilder.VerifyAll();
            mockWebHostEnvironment.VerifyAll();
        }
    }
}