using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Pages
{
    [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "*" })]
    public class ResponseCacheModel : PageModel
    {
        public DateTime DateTime { get; private set; }

        public void OnGet()
        {
            DateTime = DateTime.Now;
        }
    }
}
