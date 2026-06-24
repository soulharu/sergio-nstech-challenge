using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
