using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
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
        public void Write_WhenHttpContextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var healthReport = new HealthReport(
                new Dictionary<string, HealthReportEntry>(),
                HealthStatus.Healthy,
                TimeSpan.MaxValue
            );

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _writer.Write(null, healthReport).Wait()
            );

            Assert.Equal("context", exception.ParamName);
        }

        [Fact]
        public void Write_WhenHealthReportIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _writer.Write(mockHttpContext.Object, null).Wait()
            );

            Assert.Equal("healthReport", exception.ParamName);
        }

        public static IEnumerable<object[]> WriteJsonMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new Dictionary<string, HealthReportEntry>(),
                    null,
                    null,
                    "{\"status\":\"Unhealthy\",\"results\":[],\"totalDuration\":0}"
                },

                new object[]
                {
                    new Dictionary<string, HealthReportEntry>(),
                    HealthStatus.Unhealthy,
                    null,
                    "{\"status\":\"Unhealthy\",\"results\":[],\"totalDuration\":0}"
                },
                new object[]
                {
                    new Dictionary<string, HealthReportEntry>(),
                    HealthStatus.Degraded,
                    null,
                    "{\"status\":\"Degraded\",\"results\":[],\"totalDuration\":0}"
                },
                new object[]
                {
                    new Dictionary<string, HealthReportEntry>(),
                    HealthStatus.Healthy,
                    null,
                    "{\"status\":\"Healthy\",\"results\":[],\"totalDuration\":0}"
                },
                new object[]
                {
                    new Dictionary<string, HealthReportEntry>
                    {
                        {
                            "Test Key",
                            new HealthReportEntry(
                                HealthStatus.Healthy,
                                "Test Description",
                                new TimeSpan(0, 0, 0, 0, 100),
                                new ArgumentNullException(),
                                null
                            )
                        }
                    },
                    HealthStatus.Healthy,
                    new TimeSpan(0, 0, 0, 0, 100),
                    "{" +
                    "\"status\":\"Healthy\"," +
                    "\"results\":[{\"key\":\"Test Key\",\"value\":{\"status\":\"Healthy\",\"description\":\"Test Description\"}}]," +
                    "\"totalDuration\":100" +
                    "}"
                }
            };

        [Theory]
        [MemberData(nameof(WriteJsonMemberData))]
        public void Write_Success(
            Dictionary<string, HealthReportEntry> entries,
            HealthStatus status,
            TimeSpan totalDuration,
            string expectedResponseBody)
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            var healthReport = new HealthReport(
                entries,
                status,
                totalDuration
            );

            // Act
            _writer.Write(httpContext, healthReport).Wait();

            // Assert
            Assert.Equal("application/json", httpContext.Response.ContentType);

            httpContext.Response.Body.Position = 0;
            using var streamReader = new StreamReader(httpContext.Response.Body);
            var responseBody = streamReader.ReadToEnd();
            Assert.Equal(expectedResponseBody, responseBody);
        }
    }
}