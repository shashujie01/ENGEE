using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnGee.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class HelloApiController : ControllerBase
        {
            [HttpGet]
            public IActionResult Get()
            {
                return Ok("HELLO");
            }
        }
    
}
