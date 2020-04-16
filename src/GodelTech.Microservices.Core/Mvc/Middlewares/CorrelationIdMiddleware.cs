using System;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Http;

namespace GodelTech.Microservices.Core.Mvc.Middlewares
{
    public class CorrelationIdMiddleware
    {
        internal const string CorrelationIdHeaderName = "X-Rie-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ICorrelationIdSetter _correlationIdSetter;
        private readonly IGuidFactory _guidFactory;

        public CorrelationIdMiddleware(
            RequestDelegate next, 
            ICorrelationIdSetter correlationIdSetter,
            IGuidFactory guidFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _correlationIdSetter = correlationIdSetter ?? throw new ArgumentNullException(nameof(correlationIdSetter));
            _guidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);

            using (_correlationIdSetter.SetCorrelationId(correlationId))
            {
                context.Response.Headers.Add(CorrelationIdHeaderName, new[] { correlationId });

                await _next.Invoke(context);
            }
        }

        private string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(CorrelationIdHeaderName))
                return context.Request.Headers[CorrelationIdHeaderName];
            return _guidFactory.NewAsString();
        }
    }
}
