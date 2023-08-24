using System;
using System.Threading.Tasks;
using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [Route("api/fakes")]
    [ApiController]
    public class ApiController : FakeControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ApiController(
            IFakeService fakeService,
            IMemoryCache memoryCache,
            IMapper mapper)
            : base(fakeService, mapper)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("responseCache")]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "*" })]
        [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        public IActionResult GetResponseCache()
        {
            return Ok(DateTime.Now);
        }

        [HttpGet("memoryCache")]
        [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMemoryCache()
        {
            var currentDateTime = DateTime.Now;

            var cacheValue = await _memoryCache.GetOrCreateAsync(
                "_Current_DateTime",
                async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    return await Task.FromResult(currentDateTime);
                }
            );

            return Ok(cacheValue);
        }

        [HttpGet("fileTooLargeException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status413RequestEntityTooLarge)]
        public IActionResult GetFileTooLargeException()
        {
            FakeService.ThrowFileTooLargeException();

            return Ok();
        }

        [HttpGet("requestValidationException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status400BadRequest)]
        public IActionResult GetRequestValidationException()
        {
            FakeService.ThrowRequestValidationException();

            return Ok();
        }

        [HttpGet("resourceNotFoundException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status404NotFound)]
        public IActionResult GetResourceNotFoundException()
        {
            FakeService.ThrowResourceNotFoundException();

            return Ok();
        }
    }
}
