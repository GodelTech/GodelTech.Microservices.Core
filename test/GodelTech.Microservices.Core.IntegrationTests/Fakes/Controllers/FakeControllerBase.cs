using System.Collections.Generic;
using AutoMapper;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Controllers
{
    [Controller]
    public abstract class FakeControllerBase : ControllerBase
    {
        protected FakeControllerBase(
            IFakeService fakeService,
            IMapper mapper)
        {
            FakeService = fakeService;
            Mapper = mapper;
        }

        protected IFakeService FakeService { get; }

        protected IMapper Mapper { get; }

        [HttpGet]
        [ProducesResponseType(typeof(IList<FakeModel>), StatusCodes.Status200OK)]
        public IActionResult GetList()
        {
            return Ok(
                Mapper.Map<IList<FakeModel>>(
                    FakeService.GetList()
                )
            );
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            var item = FakeService.Get(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(
                Mapper.Map<FakeModel>(item)
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status201Created)]
        public IActionResult Post([FromBody] FakePostModel model)
        {
            var item = Mapper.Map<FakeModel>(
                FakeService.Add(model)
            );

            return CreatedAtAction(
                nameof(Get),
                new { id = item.Id },
                item
            );
        }

        [HttpGet("argumentException")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetArgumentException()
        {
            FakeService.ThrowArgumentException(null);

            return Ok();
        }

        [HttpPost("argumentException")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(FakeModel), StatusCodes.Status201Created)]
        public IActionResult PostThrowArgumentException([FromBody] FakePostModel model)
        {
            var item = Mapper.Map<FakeModel>(
                FakeService.Add(model)
            );

            FakeService.ThrowArgumentException(null);

            return CreatedAtAction(
                nameof(Get),
                new { id = item.Id },
                item
            );
        }
    }
}
