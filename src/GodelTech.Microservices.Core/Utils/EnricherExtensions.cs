using Serilog;
using Serilog.Configuration;

namespace GodelTech.Microservices.Core.Utils
{
    public static class EnricherExtensions
    {
        public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new CorrelationIdEnricher());
        }
    }
}