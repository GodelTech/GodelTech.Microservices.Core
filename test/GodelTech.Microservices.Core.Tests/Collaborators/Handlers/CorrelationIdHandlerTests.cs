using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Core.Collaborators.Handlers;
using GodelTech.Microservices.Core.Services;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Collaborators.Handlers
{
    public class CorrelationIdHandlerTests
    {
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private readonly CorrelationIdHandler _underTest;
        private readonly HttpMessageInvoker _invoker;
        private readonly HttpRequestMessage _request;

        public CorrelationIdHandlerTests()
        {
            _correlationIdAccessor = A.Fake<ICorrelationIdAccessor>();

            _underTest = new CorrelationIdHandler(_correlationIdAccessor)
            {
                InnerHandler = new DummyInnerHandler()
            };

            _invoker = new HttpMessageInvoker(_underTest);
            _request = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");
        }

        [Fact]
        public async Task SendAsync_Should_Add_CorrelationId_To_Request()
        {
            A.CallTo(() => _correlationIdAccessor.GetCorrelationId()).Returns("Id123");

            await _invoker.SendAsync(_request, new CancellationToken());

            _request.Headers.GetValues("X-Rie-Correlation-Id").Single().Should().Be("Id123");
        }

        public class DummyInnerHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK), cancellationToken);
            }
        }
    }
}
