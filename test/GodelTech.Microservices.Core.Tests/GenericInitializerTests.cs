using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests
{
    public class GenericInitializerTests
    {
        private readonly Mock<Action<IServiceCollection>> _mockConfigureServices;
        private readonly Mock<Action<IApplicationBuilder, IWebHostEnvironment>> _mockConfigure;

        public GenericInitializerTests()
        {
            _mockConfigureServices = new Mock<Action<IServiceCollection>>(MockBehavior.Strict);
            _mockConfigure = new Mock<Action<IApplicationBuilder, IWebHostEnvironment>>(MockBehavior.Strict);
        }

        [Fact]
        public void ConfigureServices_WhenNull()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);

            var initializer = new GenericInitializer(
                null,
                _mockConfigure.Object
            );

            // Act & Assert
            initializer.ConfigureServices(mockServiceCollection.Object);

            // Assert
            Assert.NotNull(initializer);
        }

        [Fact]
        public void ConfigureServices_Success()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);

            _mockConfigureServices
                .Setup(x => x.Invoke(mockServiceCollection.Object));

            var initializer = new GenericInitializer(
                _mockConfigureServices.Object
            );

            // Act & Assert
            initializer.ConfigureServices(mockServiceCollection.Object);

            // Assert
            _mockConfigureServices
                .Verify(
                    x => x.Invoke(mockServiceCollection.Object),
                    Times.Once
                );
        }

        [Fact]
        public void Configure_WhenNull()
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            var initializer = new GenericInitializer(
                _mockConfigureServices.Object,
                null
            );

            // Act & Assert
            initializer.Configure(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            Assert.NotNull(initializer);
        }

        [Fact]
        public void Configure_Success()
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            _mockConfigure
                .Setup(
                    x => x.Invoke(
                        mockApplicationBuilder.Object,
                        mockWebHostEnvironment.Object
                    )
                );

            var initializer = new GenericInitializer(
                null,
                _mockConfigure.Object
            );

            // Act & Assert
            initializer.Configure(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            _mockConfigure
                .Verify(
                    x => x.Invoke(
                        mockApplicationBuilder.Object,
                        mockWebHostEnvironment.Object
                    ),
                    Times.Once
                );
        }
    }
}
