using GodelTech.Microservices.Core.Mvc.CorrelationId;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [ApiController]
    [Route("correlationId")]
    public class CorrelationIdController : ControllerBase
    {
        private readonly ICorrelationIdContextAccessor _correlationIdContextAccessor;

        public CorrelationIdController(ICorrelationIdContextAccessor correlationIdContextAccessor)
        {
            _correlationIdContextAccessor = correlationIdContextAccessor;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CorrelationIdContext), StatusCodes.Status200OK)]
        public IActionResult Index()
        {
            return Ok(_correlationIdContextAccessor.CorrelationIdContext);
        }
    }
}
