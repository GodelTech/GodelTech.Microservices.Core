using System.Collections.Generic;

namespace GodelTech.Microservices.Core.Tests.Fakes
{
    public class FakeMicroserviceInitializerCollectionBase : MicroserviceInitializerCollectionBase
    {
        private readonly IEnumerable<IMicroserviceInitializer> _initializers;

        public FakeMicroserviceInitializerCollectionBase(IEnumerable<IMicroserviceInitializer> initializers)
        {
            _initializers = initializers;
        }

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