using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GodelTech.Microservices.Core.Mvc.Filters;
using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class ApiInitializerTests
    {
        private readonly FakeApiInitializer _initializer;

        public ApiInitializerTests()
        {
            _initializer = new FakeApiInitializer();
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
            var expectedFilters = new FilterCollection
            {
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.RequestEntityTooLarge,
                    typeof(FileTooLargeException)
                ),
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.BadRequest,
                    typeof(RequestValidationException)
                ),
                new HttpStatusCodeExceptionFilterAttribute(
                    HttpStatusCode.NotFound,
                    typeof(ResourceNotFoundException)
                )
            };

            var options = new MvcOptions();

            // Act
            _initializer.ExposedConfigureMvcOptions(options);

            // Assert
            Assert.NotNull(options);

            Assert.Equal(expectedFilters, options.Filters);
        }

        [Fact]
        public void ConfigureJsonOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _initializer.ExposedConfigureJsonOptions(null)
            );

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void ConfigureJsonOptions_Success()
        {
            // Arrange
            var expectedConverters = new List<JsonConverter>
            {
                new JsonStringEnumConverter()
            };

            var options = new JsonOptions();

            // Act
            _initializer.ExposedConfigureJsonOptions(options);

            // Assert
            Assert.NotNull(options);

            Assert.Equal(JsonNamingPolicy.CamelCase, options.JsonSerializerOptions.PropertyNamingPolicy);
            Assert.Equal(JsonNamingPolicy.CamelCase, options.JsonSerializerOptions.DictionaryKeyPolicy);
            Assert.True(options.JsonSerializerOptions.IgnoreNullValues);

            var actualConverters = options.JsonSerializerOptions.Converters;
            Assert.Equal(expectedConverters.Count, actualConverters.Count);
            for (var i = 0; i < actualConverters.Count; i++)
            {
                // this is because not possible to compare JsonConverter
                Assert.Equal(expectedConverters[i].GetType(), actualConverters[i].GetType());
            }
        }
    }
}
