using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.CorrelationId;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc.CorrelationId;
using GodelTech.Microservices.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.CorrelationId
{
    public class CorrelationIdMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockRequestDelegate;
        private readonly Mock<ICorrelationIdContextFactory> _mockCorrelationIdContextFactory;

        private readonly CorrelationIdMiddleware _middleware;

        public CorrelationIdMiddlewareTests()
        {
            _mockRequestDelegate = new Mock<RequestDelegate>(MockBehavior.Strict);

            var options = new CorrelationIdOptions();

            var mockOptions = new Mock<IOptions<CorrelationIdOptions>>(MockBehavior.Strict);
            mockOptions
                .Setup(x => x.Value)
                .Returns(options);

            _mockCorrelationIdContextFactory = new Mock<ICorrelationIdContextFactory>(MockBehavior.Strict);

            var mockGuid = new Mock<IGuid>(MockBehavior.Strict);
            mockGuid
                .Setup(x => x.NewGuid())
                .Returns(new Guid("00000000-0000-0000-0000-000000000001"));

            _middleware = new CorrelationIdMiddleware(
                _mockRequestDelegate.Object,
                mockOptions.Object,
                _mockCorrelationIdContextFactory.Object,
                mockGuid.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new CorrelationIdMiddleware(
                    _mockRequestDelegate.Object,
                    null,
                    _mockCorrelationIdContextFactory.Object
                )
            );

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _middleware.InvokeAsync(null)
            );

            Assert.Equal("context", exception.ParamName);
        }

        public static IEnumerable<object[]> InvokeAsyncMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>(),
                    new Collection<KeyValuePair<string, StringValues>>(),
                    "00000000-0000-0000-0000-000000000001",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000001")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>(),
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue")
                    },
                    "00000000-0000-0000-0000-000000000001",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue"),
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000001")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>(),
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000002")
                    },
                    "00000000-0000-0000-0000-000000000001",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000002")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue")
                    },
                    new Collection<KeyValuePair<string, StringValues>>(),
                    "00000000-0000-0000-0000-000000000001",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000001")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue"),
                        new KeyValuePair<string, StringValues>("secondKey", "SecondTestValue")
                    },
                    new Collection<KeyValuePair<string, StringValues>>(),
                    "00000000-0000-0000-0000-000000000001",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000001")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "10000000-0000-0000-0000-000000000000")
                    },
                    new Collection<KeyValuePair<string, StringValues>>(),
                    "10000000-0000-0000-0000-000000000000",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "10000000-0000-0000-0000-000000000000")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "10000000-0000-0000-0000-000000000000")
                    },
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue")
                    },
                    "10000000-0000-0000-0000-000000000000",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("firstKey", "FirstTestValue"),
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "10000000-0000-0000-0000-000000000000")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "10000000-0000-0000-0000-000000000000")
                    },
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000002")
                    },
                    "10000000-0000-0000-0000-000000000000",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000002")
                    }
                },
                new object[]
                {
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("requestFirstKey", "RequestFirstTestValue"),
                        new KeyValuePair<string, StringValues>(
                            "X-Correlation-ID",
                            new StringValues(
                                new []
                                {
                                    "00000000-0000-0000-0000-000000000002",
                                    "00000000-0000-0000-0000-000000000003"
                                }
                            )
                        )
                    },
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("responseFirstKey", "ResponseFirstTestValue")
                    },
                    "00000000-0000-0000-0000-000000000002",
                    new Collection<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("responseFirstKey", "ResponseFirstTestValue"),
                        new KeyValuePair<string, StringValues>("X-Correlation-ID", "00000000-0000-0000-0000-000000000002")
                    }
                }
            };

        [Theory]
        [MemberData(nameof(InvokeAsyncMemberData))]
        public async Task InvokeAsync_Success(
            [NotNull] ICollection<KeyValuePair<string, StringValues>> requestHeaders,
            [NotNull] ICollection<KeyValuePair<string, StringValues>> responseHeaders,
            string expectedRequestCorrelationId,
            ICollection<KeyValuePair<string, StringValues>> expectedCorrelationIdResponseHeaders)
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            // unit test context.Response.OnStarting
            // https://www.titanwolf.org/Network/q/19bae63a-0597-4517-8013-11b5547e3d3d/y
            var httpResponseFeature = new FakeResponseFeature();
            httpContext.Features.Set<IHttpResponseFeature>(httpResponseFeature);

            foreach (var requestHeader in requestHeaders)
            {
                httpContext.Request.Headers.Add(
                    requestHeader.Key,
                    requestHeader.Value
                );
            }

            foreach (var responseHeader in responseHeaders)
            {
                httpContext.Response.Headers.Add(
                    responseHeader.Key,
                    responseHeader.Value
                );
            }

            var correlationIdContext = new CorrelationIdContext("TestCorrelationId");

            _mockCorrelationIdContextFactory
                .Setup(x => x.Create(expectedRequestCorrelationId))
                .Returns(correlationIdContext);

            _mockRequestDelegate
                .Setup(x => x.Invoke(httpContext))
                .Returns(httpResponseFeature.InvokeCallBack);

            _mockCorrelationIdContextFactory
                .Setup(x => x.Clear(correlationIdContext));

            // Act
            await _middleware.InvokeAsync(httpContext);

            // Assert
            _mockCorrelationIdContextFactory
                .Verify(
                    x => x.Create(expectedRequestCorrelationId),
                    Times.Once
                );

            _mockRequestDelegate
                .Verify(
                    x => x.Invoke(httpContext),
                    Times.Once
                );

            _mockCorrelationIdContextFactory
                .Verify(
                    x => x.Clear(correlationIdContext),
                    Times.Once
                );

            // unit test context.Response.OnStarting
            var actualResponseHeaders = httpContext.Response.Headers;

            Assert.Equal(
                expectedCorrelationIdResponseHeaders,
                actualResponseHeaders
            );
        }
    }
}
