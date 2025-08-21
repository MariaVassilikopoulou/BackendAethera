using Microsoft.AspNetCore.Mvc;

namespace Aethera.Controllers
{
    [ApiController]
    [Route("/")] 
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Aethera API is live");
        }
    }
}
