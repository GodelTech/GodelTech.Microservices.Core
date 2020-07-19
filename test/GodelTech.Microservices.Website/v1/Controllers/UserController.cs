using Microsoft.AspNetCore.Mvc;

namespace GodelTech.Microservices.Website.v1.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public ActionResult ListAllAsync()
        {
            return Ok("Hello World!");
        }
    }
}
