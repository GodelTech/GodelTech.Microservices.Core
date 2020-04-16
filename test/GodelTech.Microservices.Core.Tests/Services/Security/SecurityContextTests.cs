using System;
using System.Linq;
using System.Security.Claims;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Core.Mvc.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Services.Security
{
    public sealed class SecurityContextTests
    {
        private readonly SecurityContext _underTest;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _context;

        public SecurityContextTests()
        {
            _context = A.Fake<HttpContext>();

            _httpContextAccessor = A.Fake<IHttpContextAccessor>();

            A.CallTo(() => _httpContextAccessor.HttpContext).Returns(_context);
            A.CallTo(() => _context.User).Returns(CreatePrincipal());

            _underTest = new SecurityContext(_httpContextAccessor);
        }

        [Fact]
        public void TenantId_When_Claim_Not_Found_Should_Return_Null()
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b"));

            _underTest.HasPermissions("c").Should().BeFalse();
        }

        [Fact]
        public void TenantId_When_Claim_Found_Should_Return_Expected_Result()
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal(12345));

            _underTest.TenantId.Should().Be(12345);
        }

        [Fact]
        public void HasPermissions_When_Context_Is_Not_Available_Should_Return_False()
        {
            A.CallTo(() => _httpContextAccessor.HttpContext).Returns(null);

            _underTest.HasPermissions("c").Should().BeFalse();
        }

        [Fact]
        public void HasPermissions_When_No_Matching_Permissions_Should_Return_False()
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b"));

            _underTest.HasPermissions("c").Should().BeFalse();
        }

        [Fact]
        public void HasPermissions_When_Partial_Match_Should_Return_False()
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b", "c", "d"));

            _underTest.HasPermissions("c", "d", "e").Should().BeFalse();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a", "b")]
        [InlineData("a", "b", "c")]
        [InlineData("a", "b", "c", "d")]
        public void HasPermissions_When_Full_Match_Should_Return_True(params string[] values)
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b", "c", "d"));

            _underTest.HasPermissions(values).Should().BeTrue();
        }

        [Fact]
        public void HasAnyPermission_When_Context_Is_Not_Available_Should_Return_False()
        {
            A.CallTo(() => _httpContextAccessor.HttpContext).Returns(null);

            _underTest.HasAnyPermission("c").Should().BeFalse();
        }

        [Fact]
        public void HasAnyPermission_When_No_Matching_Permissions_Should_Return_False()
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b"));

            _underTest.HasAnyPermission("c").Should().BeFalse();
        }

        [Theory]
        [InlineData(true, "a")]
        [InlineData(false, "m")]
        [InlineData(true, "m", "a", "x")]
        [InlineData(false, "m", "y", "x")]
        [InlineData(true, "a", "b")]
        [InlineData(true, "a", "b", "c")]
        [InlineData(true, "a", "b", "c", "d")]
        public void HasAnyPermission_Should_Return_Expected_Result(bool expectedResult, params string[] values)
        {
            A.CallTo(() => _context.User).Returns(CreatePrincipal("a", "b", "c", "d"));

            _underTest.HasAnyPermission(values).Should().Be(expectedResult);
        }

        [Fact]
        public void Impersonate_When_Claims_Are_Null_ShouldThrowException()
        {
            // act
            Action act = () => _underTest.Impersonate(null);
            
            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Impersonate_When_Empty_Array_Is_Passed_ShouldIgnoreContextClaims()
        {
            // arrange
            var claims = Array.Empty<Claim>();

            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));
            
            // act
            using (_underTest.Impersonate(claims))
            {
                var actualTenantId = _underTest.TenantId;
                
                // assert
                actualTenantId.Should().BeNull();
            }
        }

        [Fact]
        public void Impersonate_When_Claims_Are_Present_ShouldTakeImpersonatedClaims()
        {
            // arrange
            var tenantId = 81231;
            
            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));
            
            // act
            using (_underTest.Impersonate(new Claim("tenant_id", tenantId.ToString())))
            {
                var actual = _underTest.TenantId;
                
                // assert
                actual.Should().Be(tenantId);
            }
        }

        [Fact]
        public void Impersonate_When_Impersonate_Is_Disposed_ShouldResumeTakingClaimsFromContext()
        {
            // arrange
            var tenantId = 81231;
            
            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));

            var expected = 91823;
            
            // act
            using (_underTest.Impersonate(new Claim("tenant_id", tenantId.ToString())))
            {
            }

            var actual = _underTest.TenantId;

            // assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Impersonate_When_Different_Contexts_Are_Used_Should_Preserve_Impersonated_Claims()
        {
            // arrange
            var tenantId = 812371;

            var otherContext = A.Fake<HttpContext>();
            var otherAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => otherAccessor.HttpContext).Returns(otherContext);
            A.CallTo(() => otherContext.User).Returns(CreatePrincipal(2291233));
            
            var otherUnderTest = new SecurityContext(otherAccessor);
            
            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));
            
            // act
            using (_underTest.Impersonate(new Claim("tenant_id", tenantId.ToString())))
            {
                var actual = otherUnderTest.TenantId;

                // assert
                actual.Should().Be(tenantId);
            }
        }

        [Fact]
        public void Impersonate_When_Nested_Impersonate_Is_Called_Should_Setup_Claims_Correctly()
        {
            // arrange
            var innerTenantId = 81231;
            var outerTenantId = 337123123;
            
            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));

            // act
            using (_underTest.Impersonate(new Claim("tenant_id", outerTenantId.ToString())))
            {
                using (_underTest.Impersonate(new Claim("tenant_id", innerTenantId.ToString())))
                {
                    var actual = _underTest.TenantId;

                    // assert
                    actual.Should().Be(innerTenantId);
                }
            }
        }

        [Fact]
        public void Impersonate_When_Nested_Impersonate_Is_Released_Should_Return_Tenant_Correctly()
        {
            // arrange
            var innerTenantId = 81231;
            var outerTenantId = 337123123;
            
            A.CallTo(() => _context.User).Returns(CreatePrincipal(91823));

            // act
            using (_underTest.Impersonate(new Claim("tenant_id", outerTenantId.ToString())))
            {
                using (_underTest.Impersonate(new Claim("tenant_id", innerTenantId.ToString())))
                {
                }
                
                var actual = _underTest.TenantId;

                // assert
                actual.Should().Be(outerTenantId);
            }
        }

        private static ClaimsPrincipal CreatePrincipal(params string[] permissions)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(permissions.Select(x => new Claim("p", x))));
        }

        private static ClaimsPrincipal CreatePrincipal(int tenantId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new [] { new Claim("tenant_id", tenantId.ToString()) }));
        }
    }
}

