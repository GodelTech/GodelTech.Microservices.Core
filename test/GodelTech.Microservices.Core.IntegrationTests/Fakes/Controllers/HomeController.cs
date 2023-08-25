using System;
using System.Threading.Tasks;
using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFakeService _fakeService;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public HomeController(
            IFakeService fakeService,
            IMemoryCache memoryCache,
            IMapper mapper)
        {
            _fakeService = fakeService;
            _memoryCache = memoryCache;
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

        [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "*" })]
        public IActionResult ResponseCache()
        {
            return View(Guid.NewGuid());
        }

        public async Task<IActionResult> MemoryCache()
        {
            var value = Guid.NewGuid();

            var cacheValue = await _memoryCache.GetOrCreateAsync(
                "_Current_Guid",
                async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    return await Task.FromResult(value);
                }
            );

            return View(cacheValue);
        }

        public async Task<IActionResult> TestAsync()
        {
            await _fakeService.CompleteAsync();

            return View();
        }

        public IActionResult Route()
        {
            return Ok(
                Url.RouteUrl(
                    "default",
                    new
                    {
                        controller = "Home",
                        action = "Details",
                        id = 123
                    }
                )
            );
        }
    }
}
