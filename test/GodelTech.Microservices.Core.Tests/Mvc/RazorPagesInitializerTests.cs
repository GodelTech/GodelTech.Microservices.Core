using GodelTech.Microservices.Core.Tests.Fakes.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Mvc
{
    public class RazorPagesInitializerTests
    {
        private readonly FakeRazorPagesInitializer _initializer;

        public RazorPagesInitializerTests()
        {
            _initializer = new FakeRazorPagesInitializer();
        }

        [Fact]
        public void ConfigureRazorPagesOptions_Success()
        {
            // Arrange
            var mockOptions = new Mock<RazorPagesOptions>(MockBehavior.Strict);

            // Act
            _initializer.ExposedConfigureRazorPagesOptions(mockOptions.Object);

            // Assert
            Assert.NotNull(mockOptions.Object);

            mockOptions.VerifyAll();
        }

        [Fact]
        public void ConfigureMvcBuilder_Success()
        {
            // Arrange
            var mockMvcBuilder = new Mock<IMvcBuilder>(MockBehavior.Strict);

            // Act
            _initializer.ExposedConfigureMvcBuilder(mockMvcBuilder.Object);

            // Assert
            Assert.NotNull(mockMvcBuilder.Object);

            mockMvcBuilder.VerifyAll();
        }
    }
}