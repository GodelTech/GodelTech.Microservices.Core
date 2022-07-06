using GodelTech.Microservices.Core.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc
{
    public class FakeRazorPagesInitializer : RazorPagesInitializer
    {
        public void ExposedConfigureRazorPagesOptions(RazorPagesOptions options)
        {
            base.ConfigureRazorPagesOptions(options);
        }

        public virtual void ExposedConfigureMvcBuilder(IMvcBuilder builder)
        {
            base.ConfigureMvcBuilder(builder);
        }
    }
}
