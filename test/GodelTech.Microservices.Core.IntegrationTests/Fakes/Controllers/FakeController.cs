using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [Route("fakes")]
    [ApiController]
    public class FakeController : FakeControllerBase
    {
        public FakeController(
            IFakeService fakeService,
            IMapper mapper)
            : base(fakeService, mapper)
        {

        }
    }
}
