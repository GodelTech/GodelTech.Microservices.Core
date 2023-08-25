using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Pages
{
    [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "*" })]
    public class ResponseCacheModel : PageModel
    {
        public Guid Value { get; private set; }

        public void OnGet()
        {
            Value = Guid.NewGuid();
        }
    }
}
