using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Core.Collaborators.Handlers;
using GodelTech.Microservices.Core.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Collaborators.Handlers
{
    public sealed class ClientCredentialsAuthTokenHandlerTests
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<ClientCredentialsAuthTokenHandler> _logger;
        private readonly IBearerTokenStorage _bearerTokenStorage;
        private readonly HttpMessageInvoker _invoker;

        private readonly ClientCredentialsAuthTokenHandler _underTest;
        
        public ClientCredentialsAuthTokenHandlerTests()
        {
            _identityService = A.Fake<IIdentityService>();
            _logger = A.Fake<ILogger<ClientCredentialsAuthTokenHandler>>();
            _bearerTokenStorage = A.Fake<IBearerTokenStorage>();
            
            _underTest = new ClientCredentialsAuthTokenHandler(_logger, _identityService, _bearerTokenStorage)
            {
                InnerHandler = new CorrelationIdHandlerTests.DummyInnerHandler()
            };
            
            _invoker = new HttpMessageInvoker(_underTest);
        }

        [Fact]
        public void SendAsync_WhenRequestIsNull_ShouldThrowException()
        {
            // act
            Func<Task> act = () => _invoker.SendAsync(null, CancellationToken.None);
            
            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public async Task SendAsync_WhenAuthHeaderAlreadyExists_ShouldSkipAuthInitialization()
        {
            // arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://something.com");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "something_wicked");
            
            // act
            var actual = await _invoker.SendAsync(request, CancellationToken.None);
            
            // assert
            actual.Should().NotBeNull();
            request.Headers.Authorization.Should().BeEquivalentTo(new AuthenticationHeaderValue("bearer", "something_wicked"));
        }
        
        [Fact]
        public async Task SendAsync_WhenRequestedTokenIsPresent_ShouldSetupTokenCorrectly()
        {
            // arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://something.com");
            var token = "something_really_wicked";

            A.CallTo(() => _identityService.GetClientCredentialsTokenAsync()).Returns(Task.FromResult(token));
            A.CallTo(() => _bearerTokenStorage.SetAccessToken(token)).Returns(new DisposableAction(() => { }));
            
            // act
            var actual = await _invoker.SendAsync(request, CancellationToken.None);
            
            // assert
            actual.Should().NotBeNull();
            A.CallTo(() => _bearerTokenStorage.SetAccessToken(token)).MustHaveHappened(1, Times.Exactly);
        }
    }
}