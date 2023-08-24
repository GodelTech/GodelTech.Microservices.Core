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

        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "*" })]
        public IActionResult ResponseCache()
        {
            return View(DateTime.Now);
        }

        public async Task<IActionResult> MemoryCache()
        {
            var currentDateTime = DateTime.Now;

            var cacheValue = await _memoryCache.GetOrCreateAsync(
                "_Current_DateTime",
                async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    return await Task.FromResult(currentDateTime);
                }
            );

            return View(cacheValue);
        }

        public async Task<IActionResult> TestAsync()
        {
            await _fakeService.CompleteAsync();

            return View();
        }
    }
}
