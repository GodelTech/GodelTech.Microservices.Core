using System.Collections.Generic;
using GodelTech.Microservices.Core.Tests.Fakes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests
{
    public class MicroserviceInitializerCollectionBaseTests
    {
        private readonly IList<IMicroserviceInitializer> _mockInitializers;

        private readonly FakeMicroserviceInitializerCollectionBase _initializerCollection;

        public MicroserviceInitializerCollectionBaseTests()
        {
            _mockInitializers = new List<IMicroserviceInitializer>();

            _initializerCollection = new FakeMicroserviceInitializerCollectionBase(
                _mockInitializers
            );
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
            _initializerCollection.ConfigureServices(mockServiceCollection.Object);

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

            for (var i = 0; i < count; i++)
            {
                _mockInitializers.Add(mockInitializer.Object);
            }

            // Act
            _initializerCollection.Configure(
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ConfigureEndpoints_Success(int count)
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            var mockInitializer = new Mock<IMicroserviceInitializer>(MockBehavior.Strict);
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
            _initializerCollection.ConfigureEndpoints(
                mockApplicationBuilder.Object,
                mockWebHostEnvironment.Object
            );

            // Assert
            mockInitializer
                .Verify(
                    x => x.ConfigureEndpoints(
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
            var result = _initializerCollection.ExposedCreateInitializers();

            // Assert
            Assert.Equal(_mockInitializers, result);
        }
    }
}
