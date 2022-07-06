using System;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class MvcInitializerTests
    {
        private readonly FakeMvcInitializer _initializer;

        public MvcInitializerTests()
        {
            _initializer = new FakeMvcInitializer();
        }

        [Fact]
        public void ConfigureMvcOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureMvcOptions(null)
            );

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void ConfigureMvcOptions_Success()
        {
            // Arrange
            var options = new MvcOptions();

            // Act
            _initializer.ExposedConfigureMvcOptions(options);

            // Assert
            Assert.NotNull(options);

            Assert.False(options.SuppressAsyncSuffixInActionNames);
        }

        [Fact]
        public void ConfigureMvcBuilder_Success()
        {
            // Arrange
            var mockMvcBuilder = new Mock<IMvcBuilder>(MockBehavior.Strict);

            // Act
            _initializer.ExposedConfigureMvcBuilder(mockMvcBuilder.Object);

            // Assert
            Assert.NotNull(mockMvcBuilder.Object);

            mockMvcBuilder.VerifyAll();
        }

        [Fact]
        public void ConfigureEndpoints_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureEndpoints(null)
            );

            Assert.Equal("endpoints", exception.ParamName);
        }
    }
}
