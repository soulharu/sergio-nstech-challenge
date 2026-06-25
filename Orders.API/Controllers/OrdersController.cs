using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet(Name = "get")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
