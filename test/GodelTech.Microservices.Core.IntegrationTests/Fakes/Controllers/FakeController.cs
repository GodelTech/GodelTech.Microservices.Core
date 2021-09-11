using System.Collections.Generic;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [ApiController]
    [Route("fakes")]
    public class FakeController : ControllerBase
    {
        private readonly IFakeService _service;

        public FakeController(IFakeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<FakeModel>), StatusCodes.Status200OK)]
        public IActionResult GetList()
        {
            var list = _service.GetList();

            return Ok(list);
        }

        [HttpGet("fileTooLargeException")]
        // todo: suggest to mention ErrorModel if we agree to return it via Filter
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        public IActionResult GetFileTooLargeException()
        {
            _service.ThrowFileTooLargeException();

            return Ok();
        }

        [HttpGet("requestValidationException")]
        // todo: suggest to mention ErrorModel if we agree to return it via Filter
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetRequestValidationException()
        {
            _service.ThrowRequestValidationException();

            return Ok();
        }

        [HttpGet("resourceNotFoundException")]
        // todo: suggest to mention ErrorModel if we agree to return it via Filter
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetResourceNotFoundException()
        {
            _service.ThrowResourceNotFoundException();

            return Ok();
        }
    }
}