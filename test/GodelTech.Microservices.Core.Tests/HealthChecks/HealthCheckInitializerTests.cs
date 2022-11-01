using System;
using System.Collections.Generic;
using GodelTech.Microservices.Core.HealthChecks;
using GodelTech.Microservices.Core.Tests.Fakes.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.HealthChecks
{
    public class HealthCheckInitializerTests
    {
        private readonly Mock<IApplicationBuilder> _mockApplicationBuilder;

        private readonly FakeHealthCheckInitializer _initializer;

        public HealthCheckInitializerTests()
        {
            _mockApplicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);

            _initializer = new FakeHealthCheckInitializer();
        }

        [Fact]
        public void ConfigureHealthCheckOptions_WhenHealthCheckOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureHealthCheckOptions(
                    null,
                    _mockApplicationBuilder.Object
                )
            );

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void ConfigureHealthCheckOptions_WhenApplicationBuilderIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var options = new HealthCheckOptions();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureHealthCheckOptions(options, null)
            );

            Assert.Equal("app", exception.ParamName);
        }

        [Fact]
        public void ConfigureHealthCheckOptions_Success()
        {
            // Arrange
            var expectedResultStatusCodes = new Dictionary<HealthStatus, int>
            {
                {HealthStatus.Healthy, StatusCodes.Status200OK},
                {HealthStatus.Degraded, StatusCodes.Status200OK},
                {HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable}
            };

            var options = new HealthCheckOptions();

            var mockHealthCheckResponseWriter = new Mock<IHealthCheckResponseWriter>(MockBehavior.Strict);

            _mockApplicationBuilder
                .Setup(x => x.ApplicationServices.GetService(typeof(IHealthCheckResponseWriter)))
                .Returns(mockHealthCheckResponseWriter.Object);

            // Act
            _initializer.ExposedConfigureHealthCheckOptions(options, _mockApplicationBuilder.Object);

            // Assert
            Assert.NotNull(options);

            Assert.False(options.AllowCachingResponses);
            Assert.True(options.Predicate.Invoke(default));
            Assert.Equal(mockHealthCheckResponseWriter.Object.WriteAsync, options.ResponseWriter);
            Assert.Equal(expectedResultStatusCodes, options.ResultStatusCodes);
        }
    }
}
