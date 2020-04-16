using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Core.Collaborators.Handlers;
using GodelTech.Microservices.Core.Mvc.Security;
using GodelTech.Microservices.Core.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Collaborators.Handlers
{
    public sealed class TenantClientCredentialsAuthTokenHandlerTests
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<TenantClientCredentialsAuthTokenHandler> _logger;
        private readonly ISecurityContext _securityContext;
        private readonly IBearerTokenStorage _bearerTokenStorage;
        private readonly HttpMessageInvoker _invoker;

        private readonly TenantClientCredentialsAuthTokenHandler _underTest;
        
        public TenantClientCredentialsAuthTokenHandlerTests()
        {
            _identityService = A.Fake<IIdentityService>();
            _logger = A.Fake<ILogger<TenantClientCredentialsAuthTokenHandler>>();
            _securityContext = A.Fake<ISecurityContext>();
            _bearerTokenStorage = A.Fake<IBearerTokenStorage>();
            
            _underTest = new TenantClientCredentialsAuthTokenHandler(_logger, _securityContext, _identityService, _bearerTokenStorage)
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
        public async Task SendAsync_WhenSecurityContextDoesNotHaveTenantId_ShouldNotSetupAnyToken()
        {
            // arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://something.com");
            A.CallTo(() => _securityContext.TenantId).Returns(null);
            
            // act
            var actual = await _invoker.SendAsync(request, CancellationToken.None);
            
            // assert
            actual.Should().NotBeNull();
            A.CallTo(() => _bearerTokenStorage.SetAccessToken(A<string>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public async Task SendAsync_WhenSecurityContextHasTenantId_ShouldSetupTokenCorrectly()
        {
            // arrange
            var tenantId = 8171723;
            var request = new HttpRequestMessage(HttpMethod.Get, "http://something.com");
            A.CallTo(() => _securityContext.TenantId).Returns(tenantId);
            
            var token = "something_really_wicked";

            A.CallTo(() => _identityService.GetTenantClientCredentialsTokenAsync(tenantId)).Returns(Task.FromResult(token));
            A.CallTo(() => _bearerTokenStorage.SetAccessToken(token)).Returns(new DisposableAction(() => { }));
            
            // act
            var actual = await _invoker.SendAsync(request, CancellationToken.None);
            
            // assert
            actual.Should().NotBeNull();
            A.CallTo(() => _bearerTokenStorage.SetAccessToken(token)).MustHaveHappened(1, Times.Exactly);
        }
    }
}