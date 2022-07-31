using System;
using System.Collections.Generic;
using GodelTech.Microservices.Core.Tests.Fakes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests
{
    public class MicroserviceStartupTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly IList<IMicroserviceInitializer> _mockInitializers;

        private readonly FakeMicroserviceStartup _microserviceStartup;

        public MicroserviceStartupTests()
        {
            _mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);
            _mockInitializers = new List<IMicroserviceInitializer>();

            _microserviceStartup = new FakeMicroserviceStartup(
                _mockConfiguration.Object,
                _mockInitializers
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FakeMicroserviceStartup(
                    null,
                    _mockInitializers
                )
            );

            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void Configuration_Success()
        {
            // Arrange & Act
            var result = _microserviceStartup.ExposedConfiguration;

            // Assert
            Assert.Equal(_mockConfiguration.Object, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ConfigureServices_Success(int count)
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);

            var mockInitializer = new Mock<IMicroserviceInitializer>(MockBehavior.Strict);
            mockInitializer
                .Setup(x => x.ConfigureServices(mockServiceCollection.Object));

            for (var i = 0; i < count; i++)
            {
                _mockInitializers.Add(mockInitializer.Object);
            }

            // Act
            _microserviceStartup.ConfigureServices(mockServiceCollection.Object);

            // Assert
            mockInitializer
                .Verify(
                    x => x.ConfigureServices(mockServiceCollection.Object),
                    Times.Exactly(count)
                );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Configure_Success(int count)
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            var mockInitializer = new Mock<IMicroserviceInitializer>(MockBehavior.Strict);
            mockInitializer
                .Setup(
                    x => x.Configure(
                        mockApplicationBuilder.Object,
                        mockWebHostEnvironment.Object
                    )
                );
            mockInitializer
                .Setup(
                    x => x.ConfigureEndpoints(
                        mockApplicationBuilder.Object,
                        mockWebHostEnvironment.Object
                    )
                );

            for (var i = 0; i < count; i++)
            {
                _mockInitializers.Add(mockInitializer.Object);
            }

            // Act
            _microserviceStartup.Configure(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            mockInitializer
                .Verify(
                    x => x.Configure(
                        mockApplicationBuilder.Object,
                        mockWebHostEnvironment.Object
                    ),
                    Times.Exactly(count)
                );
        }

        [Fact]
        public void CreateInitializers_Success()
        {
            // Arrange & Act
            var result = _microserviceStartup.ExposedCreateInitializers();

            // Assert
            Assert.Equal(_mockInitializers, result);
        }
    }
}
