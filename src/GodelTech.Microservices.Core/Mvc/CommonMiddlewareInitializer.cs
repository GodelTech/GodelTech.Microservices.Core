using System.Collections.Generic;

namespace GodelTech.Microservices.Core.Mvc
{
    /// <summary>
    /// CommonMiddleware initializer.
    /// </summary>
    public class CommonMiddlewareInitializer : MicroserviceInitializerCollectionBase
    {
        /// <inheritdoc />
        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new CorrelationIdMiddlewareInitializer();
            yield return new RequestResponseLoggingMiddlewareInitializer();
            yield return new LogUncaughtErrorsMiddlewareInitializer();
        }
    }
}