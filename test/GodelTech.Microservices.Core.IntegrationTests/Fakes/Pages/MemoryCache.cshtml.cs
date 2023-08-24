using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Pages
{
    public class MemoryCacheModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public DateTime DateTime { get; private set; }

        public async Task<IActionResult> OnGetAsync()
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

            DateTime = cacheValue;

            return Page();
        }
    }
}
