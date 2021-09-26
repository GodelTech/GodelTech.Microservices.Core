using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFakeService _service;

        public HomeController(IFakeService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var item = _service.Get(id);

            return View(item);
        }

        public async Task<IActionResult> TestAsync()
        {
            await _service.CompleteAsync();

            return View();
        }
    }
}