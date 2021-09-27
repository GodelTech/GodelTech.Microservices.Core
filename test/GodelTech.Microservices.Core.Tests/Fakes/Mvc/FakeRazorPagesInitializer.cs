using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeRazorPagesInitializer : RazorPagesInitializer
    {
        public void ExposedConfigureRazorPagesOptions(RazorPagesOptions options)
        {
            base.ConfigureRazorPagesOptions(options);
        }
    }
}