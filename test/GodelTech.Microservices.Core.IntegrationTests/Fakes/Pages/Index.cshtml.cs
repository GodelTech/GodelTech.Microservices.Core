using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFakeService _service;

        public IndexModel(IFakeService service)
        {
            _service = service;
        }

        public void OnGet()
        {
            _service.GetList();
        }
    }
}
