using System.Collections.Generic;
using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using GodelTech.Microservices.Core.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [ApiController]
    [Route("fakes")]
    public class FakeController : ControllerBase
    {
        private readonly IFakeService _fakeService;
        private readonly IMapper _mapper;

        public FakeController(
            IFakeService fakeService,
            IMapper mapper)
        {
            _fakeService = fakeService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<FakeModel>), StatusCodes.Status200OK)]
        public IActionResult GetList()
        {
            return Ok(
                _mapper.Map<IList<FakeModel>>(
                    _fakeService.GetList()
                )
            );
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            var item = _fakeService.Get(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(
                _mapper.Map<FakeModel>(item)
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status201Created)]
        public IActionResult Post([FromBody] FakePostModel model)
        {
            var item = _mapper.Map<FakeModel>(
                _fakeService.Add(model)
            );

            return CreatedAtAction(
                nameof(Get),
                new { id = item.Id },
                item
            );
        }

        [HttpGet("fileTooLargeException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status413RequestEntityTooLarge)]
        public IActionResult GetFileTooLargeException()
        {
            _fakeService.ThrowFileTooLargeException();

            return Ok();
        }

        [HttpGet("requestValidationException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status400BadRequest)]
        public IActionResult GetRequestValidationException()
        {
            _fakeService.ThrowRequestValidationException();

            return Ok();
        }

        [HttpGet("resourceNotFoundException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status404NotFound)]
        public IActionResult GetResourceNotFoundException()
        {
            _fakeService.ThrowResourceNotFoundException();

            return Ok();
        }

        [HttpGet("argumentException")]
        [ProducesResponseType(typeof(ExceptionFilterResultModel), StatusCodes.Status413RequestEntityTooLarge)]
        public IActionResult GetArgumentException()
        {
            _fakeService.ThrowArgumentException(null);

            return Ok();
        }
    }
}