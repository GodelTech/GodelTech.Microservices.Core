using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Models;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Mappings
{
    public class FakeProfile : Profile
    {
        public FakeProfile()
        {
            CreateMap<FakeDto, FakeModel>();
        }
    }
}