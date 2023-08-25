﻿using System;
using System.Collections.Generic;
using System.IO;
using GodelTech.Microservices.Core.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.HealthChecks
{
    public class HealthCheckResponseWriterTests
    {
        private readonly HealthCheckResponseWriter _writer;

        public HealthCheckResponseWriterTests()
        {
            _writer = new HealthCheckResponseWriter();
        }

        [Fact]
        public void WriteAsync_WhenHttpContextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var healthReport = new HealthReport(
                new Dictionary<string, HealthReportEntry>(),
                HealthStatus.Healthy,
                TimeSpan.MaxValue
            );

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _writer.WriteAsync(null, healthReport).Wait()
            );

            Assert.Equal("context", exception.ParamName);
        }

        [Fact]
        public void WriteAsync_WhenHealthReportIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _writer.WriteAsync(mockHttpContext.Object, null).Wait()
            );

            Assert.Equal("healthReport", exception.ParamName);
        }

        [Fact]
        public void WriteAsync_Success()
        {
            // Arrange
            var entries = new Dictionary<string, HealthReportEntry>
            {
                {
                    "Test Key", new HealthReportEntry(
                        HealthStatus.Degraded,
                        "Test Description",
                        new TimeSpan(0, 0, 0, 0, 100),
                        null,
                        null
                    )
                }
            };

            var httpContext = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            var healthReport = new HealthReport(
                entries,
                HealthStatus.Healthy,
                new TimeSpan(0, 0, 0, 0, 100)
            );

            var expectedResponseBody =
                "{" +
                "\"status\":\"Healthy\"," +
                "\"results\":[{\"key\":\"Test Key\",\"value\":{\"status\":\"Degraded\",\"description\":\"Test Description\"}}]," +
                "\"totalDuration\":100" +
                "}";

            // Act
            _writer.WriteAsync(httpContext, healthReport).Wait();

            // Assert
            Assert.Equal("application/json", httpContext.Response.ContentType);

            httpContext.Response.Body.Position = 0;
            using var streamReader = new StreamReader(httpContext.Response.Body);
            var responseBody = streamReader.ReadToEnd();
            Assert.Equal(expectedResponseBody, responseBody);
        }
    }
}
