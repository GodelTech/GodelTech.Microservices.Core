using System.Threading.Tasks;
using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFakeService _fakeService;
        private readonly IMapper _mapper;

        public HomeController(
            IFakeService fakeService,
            IMapper mapper)
        {
            _fakeService = fakeService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var item = _fakeService.Get(id);

            return View(
                _mapper.Map<FakeModel>(item)
            );
        }

        public async Task<IActionResult> TestAsync()
        {
            await _fakeService.CompleteAsync();

            return View();
        }
    }
}