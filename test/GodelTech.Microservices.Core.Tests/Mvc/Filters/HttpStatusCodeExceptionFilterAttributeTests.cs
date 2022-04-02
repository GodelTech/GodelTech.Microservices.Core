using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using FluentAssertions;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc.Filters
{
    public class HttpStatusCodeExceptionFilterAttributeTests
    {
        public static IEnumerable<object[]> ConstructorMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(400, typeof(ArgumentNullException)),
                    400,
                    typeof(ArgumentNullException)
                },
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(HttpStatusCode.NotFound, typeof(ResourceNotFoundException)),
                    404,
                    typeof(ResourceNotFoundException)
                },
            };

        [Theory]
        [MemberData(nameof(ConstructorMemberData))]
        public void Constructor(
            HttpStatusCodeExceptionFilterAttribute item,
            int expectedStatusCode,
            Type expectedExceptionType)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Arrange & Act & Assert
            Assert.Equal(expectedStatusCode, item.StatusCode);
            Assert.Equal(expectedExceptionType, item.ExceptionType);
        }

        [Fact]
        public void OnException_ThrowsArgumentNullException()
        {
            // Arrange
            var attribute = new HttpStatusCodeExceptionFilterAttribute(
                HttpStatusCode.NotFound,
                typeof(ResourceNotFoundException)
            );

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => attribute.OnException(null)
            );

            Assert.Equal("context", exception.ParamName);
        }

        public static IEnumerable<object[]> OnExceptionMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(HttpStatusCode.NotFound, typeof(ResourceNotFoundException)),
                    new AggregateException(
                        new ResourceNotFoundException()
                    ),
                    404,
                    new ObjectResult(
                        new
                        {
                            ErrorMessage = $"One or more errors occurred. (Exception of type '{typeof(ResourceNotFoundException)}' was thrown.)"
                        }
                    )
                },
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(HttpStatusCode.NotFound, typeof(ResourceNotFoundException)),
                    new AggregateException(
                        new ArgumentNullException(),
                        new ResourceNotFoundException()
                    ),
                    404,
                    new ObjectResult(
                        new
                        {
                            ErrorMessage = $"One or more errors occurred. (Value cannot be null.) (Exception of type '{typeof(ResourceNotFoundException)}' was thrown.)"
                        }
                    )
                },
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(HttpStatusCode.NotFound, typeof(ResourceNotFoundException)),
                    new ArgumentNullException(),
                    200,
                    null
                },
                new object[]
                {
                    new HttpStatusCodeExceptionFilterAttribute(HttpStatusCode.NotFound, typeof(ResourceNotFoundException)),
                    new ResourceNotFoundException(),
                    404,
                    new ObjectResult(
                        new
                        {
                            ErrorMessage = $"Exception of type '{typeof(ResourceNotFoundException)}' was thrown."
                        }
                    )
                }
            };

        [Theory]
        [MemberData(nameof(OnExceptionMemberData))]
        public void OnException_Success(
            HttpStatusCodeExceptionFilterAttribute attribute,
            Exception exception,
            int expectedStatusCode,
            ObjectResult expectedResult)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            // Arrange
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };

            // Act
            attribute.OnException(exceptionContext);

            // Assert
            Assert.Equal(expectedStatusCode, exceptionContext.HttpContext.Response.StatusCode);
            exceptionContext.Result
                .Should()
                .BeEquivalentTo(expectedResult);
        }
    }
}