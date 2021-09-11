using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Tests.Fakes
{
    public class FakeMicroserviceStartup : MicroserviceStartup
    {
        private readonly IEnumerable<IMicroserviceInitializer> _initializers;

        public FakeMicroserviceStartup(
            IConfiguration configuration,
            IEnumerable<IMicroserviceInitializer> initializers)
            : base(configuration)
        {
            _initializers = initializers;
        }

        public IConfiguration ExposedConfiguration => Configuration;

        public IEnumerable<IMicroserviceInitializer> ExposedCreateInitializers()
        {
            return CreateInitializers();
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            return _initializers;
        }
    }
}