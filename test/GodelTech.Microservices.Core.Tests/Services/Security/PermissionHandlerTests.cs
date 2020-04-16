using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core.Mvc.Security;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Services.Security
{
    public class PermissionHandlerTests
    {
        private readonly PermissionHandler _underTest;

        public PermissionHandlerTests()
        {
            _underTest = new PermissionHandler();
        }

        [Fact]
        public async Task HandleAsync_When_Not_Authenticated_Should_Fail()
        {
            var context = new AuthorizationHandlerContext(
                new [] { new PermissionRequirement("test") },
                CreateUnAuthenticatedPrincipal(),
                null);

            await _underTest.HandleAsync(context);

            context.HasFailed.Should().BeTrue();
            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_When_Partial_Permission_Match_Should_Not_Fail_Not_Succeed()
        {
            var context = new AuthorizationHandlerContext(
                new[] { new PermissionRequirement("a", "b") },
                CreatePrincipal("b", "c"),
                null);

            await _underTest.HandleAsync(context);

            context.HasFailed.Should().BeFalse();
            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_When_Full_Permission_Match_Should_Fail()
        {
            var context = new AuthorizationHandlerContext(
                new[] { new PermissionRequirement("a", "b") },
                CreatePrincipal("a", "b", "c"),
                null);

            await _underTest.HandleAsync(context);

            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        private static ClaimsPrincipal CreateUnAuthenticatedPrincipal()
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        private static ClaimsPrincipal CreatePrincipal(params string[] permissions)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(
                permissions.Select(x => new Claim("p", x)),
                "test"));
        }
    }
}
