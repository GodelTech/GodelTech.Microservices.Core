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
    }
}