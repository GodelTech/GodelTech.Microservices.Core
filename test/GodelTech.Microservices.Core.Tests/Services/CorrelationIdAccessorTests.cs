using System;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Microservices.Core.Services;
using Xunit;

namespace GodelTech.Microservices.Core.Tests.Services
{
    public class CorrelationIdAccessorTests : IDisposable
    {
        private readonly CorrelationIdAccessor _underTest;

        public CorrelationIdAccessorTests()
        {
            _underTest = new CorrelationIdAccessor();
        }

        public void Dispose()
        {
            _underTest.SetCorrelationId(null);
        }

        [Fact]
        public void GetCorrelationId_When_Value_Not_Set_Should_Return_Null()
        {
            _underTest.GetCorrelationId().Should().BeNull();
        }

        [Fact]
        public void GetCorrelationId_When_Value_Set_Should_Return_Set_Value()
        {
            const string id = "id123";

            _underTest.SetCorrelationId(id);

            _underTest.GetCorrelationId().Should().Be(id);
        }

        [Fact]
        public void GetCorrelationId_When_Value_Set_Should_Reset_Value_After_It_Is_Disposed()
        {
            const string id = "id222";

            using (_underTest.SetCorrelationId(id))
            {
                _underTest.GetCorrelationId().Should().Be(id);
            }

            _underTest.GetCorrelationId().Should().BeNull();
        }

        [Fact]
        public async Task GetCorrelationId_Should_Work_Corrently_With_Async_Methods()
        {
            var task1 = TestAsyncMethod("value1");
            var task2 = TestAsyncMethod("value2");

            await task2;
            await task1;
        }

        private async Task TestAsyncMethod(string value)
        {
            _underTest.SetCorrelationId(value);

            await Task.Delay(2000);

            _underTest.GetCorrelationId().Should().Be(value);
        }

        [Fact]
        public async Task GetCorrelationId_Should_Use_The_Same_Values_For_Nested_Async_Executions()
        {
            _underTest.SetCorrelationId("id123");

            await TestAsyncMethodNoInit("id123");
        }

        private async Task TestAsyncMethodNoInit(string correlationId)
        {
            await Task.Delay(2000);

            _underTest.GetCorrelationId().Should().Be(correlationId);
        }
    }
}
