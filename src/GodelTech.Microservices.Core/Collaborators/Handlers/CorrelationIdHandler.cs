using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Mvc.Middlewares;
using GodelTech.Microservices.Core.Services;

namespace GodelTech.Microservices.Core.Collaborators.Handlers
{
    public class CorrelationIdHandler : DelegatingHandler
    {
        private readonly ICorrelationIdAccessor _correlationIdAccessor;

        public CorrelationIdHandler(ICorrelationIdAccessor correlationIdAccessor)
        {
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = _correlationIdAccessor.GetCorrelationId();

            if (!string.IsNullOrWhiteSpace(correlationId))
                request.Headers.Add(CorrelationIdMiddleware.CorrelationIdHeaderName, new[] { correlationId });

            return base.SendAsync(request, cancellationToken);
        }
    }
}
