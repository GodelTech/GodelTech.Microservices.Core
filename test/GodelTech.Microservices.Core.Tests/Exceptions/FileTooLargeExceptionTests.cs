using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Exceptions
{
    public class FileTooLargeExceptionTests
    {
        private static readonly ArgumentNullException InnerException = new ArgumentNullException();

        public static IEnumerable<object[]> ConstructorMemberData =>
            new Collection<object[]>
            {
                new object[] { null, null, null },
                new object[]
                {
                    new FileTooLargeException(),
                    $"Exception of type '{typeof(FileTooLargeException)}' was thrown.",
                    null
                },
                new object[]
                {
                    new FileTooLargeException("Test Message"),
                    "Test Message",
                    null
                },
                new object[]
                {
                    new FileTooLargeException("Test Message", InnerException),
                    "Test Message",
                    InnerException
                }
            };

        [Theory]
        [MemberData(nameof(ConstructorMemberData))]
        public void Constructor(
            FileTooLargeException item,
            string expectedMessage,
            Exception expectedInnerException)
        {
            // Arrange & Act & Assert
            Assert.Equal(expectedMessage, item?.Message);
            Assert.Equal(expectedInnerException, item?.InnerException);
        }
    }
}