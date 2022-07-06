using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Tests.Fakes
{
    public class FakeMicroserviceInitializerBase : MicroserviceInitializerBase
    {
        public FakeMicroserviceInitializerBase(IConfiguration configuration)
            : base(configuration)
        {

        }

        public IConfiguration ExposedConfiguration => Configuration;
    }
}
