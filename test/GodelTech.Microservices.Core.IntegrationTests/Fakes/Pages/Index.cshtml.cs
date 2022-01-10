using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFakeService _fakeService;

        public IndexModel(IFakeService fakeService)
        {
            _fakeService = fakeService;
        }

        public void OnGet()
        {
            _fakeService.GetList();
        }
    }
}
