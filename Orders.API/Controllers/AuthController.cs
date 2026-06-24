using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
