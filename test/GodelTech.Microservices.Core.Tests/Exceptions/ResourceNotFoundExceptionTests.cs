using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Exceptions
{
    public class ResourceNotFoundExceptionTests
    {
        private static readonly ArgumentNullException InnerException = new ArgumentNullException();

        public static IEnumerable<object[]> ConstructorMemberData =>
            new Collection<object[]>
            {
                new object[] { null, null, null },
                new object[]
                {
                    new ResourceNotFoundException(),
                    $"Exception of type '{typeof(ResourceNotFoundException)}' was thrown.",
                    null
                },
                new object[]
                {
                    new ResourceNotFoundException("Test Message"),
                    "Test Message",
                    null
                },
                new object[]
                {
                    new ResourceNotFoundException("Test Message", InnerException),
                    "Test Message",
                    InnerException
                }
            };

        [Theory]
        [MemberData(nameof(ConstructorMemberData))]
        public void Constructor(
            ResourceNotFoundException item,
            string expectedMessage,
            Exception expectedInnerException)
        {
            // Arrange & Act & Assert
            Assert.Equal(expectedMessage, item?.Message);
            Assert.Equal(expectedInnerException, item?.InnerException);
        }
    }
}