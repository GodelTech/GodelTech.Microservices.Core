using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Core.Collaborators;
using GodelTech.Microservices.Core.Collaborators.ResponseHandlers;
using GodelTech.Microservices.Core.Services;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Collaborators
{
    public class ContentHandlerFactoryTests
    {
        private readonly ResponseHandlerFactory _underTest;

        public ContentHandlerFactoryTests()
        {
            _underTest = new ResponseHandlerFactory(A.Fake<IJsonSerializer>());
        }

        [Theory]
        [InlineData(typeof(string), typeof(StringResponseHandler))]
        [InlineData(typeof(Stream), typeof(StreamResponseHandler))]
        [InlineData(typeof(byte[]), typeof(ByteResponseHandler))]
        [InlineData(typeof(ContentHandlerFactoryTests), typeof(DefaultResponseHandler))]
        public void Create_Should_Create_Handler_Of_Expected_Type(Type dataType, Type expectedHandlerType)
        {
            _underTest.Create(dataType).Should().BeOfType(expectedHandlerType);
        }
    }
}
