using System;
using GodelTech.Microservices.Core.Services;
using Serilog.Core;
using Serilog.Events;

namespace GodelTech.Microservices.Core.Utils
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            var correlationId = CorrelationIdAccessor.CurrentCorrelationId;

            if (!string.IsNullOrWhiteSpace(correlationId))
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RieCorrelationId", correlationId));
        }
    }
}
