using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake
{
    public class FakePostModel : IFakeAddDto
    {
        public string ServiceName { get; set; }

        public string Message { get; set; }
    }
}