using System.Collections.Generic;
using GodelTech.Microservices.Core.Mvc;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeCommonMiddlewareInitializer : CommonMiddlewareInitializer
    {
        public IEnumerable<IMicroserviceInitializer> ExposedCreateInitializers()
        {
            return CreateInitializers();
        }
    }
}
