using Microsoft.AspNetCore.Mvc;
using Orders.API.Contracts.Authentication;
using Orders.Domain.Interfaces.Services;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public AuthController(IAuthService authenticationService) => _authenticationService = authenticationService;


        [HttpPost(Name = "token")]
        public IActionResult GetToken([FromBody] TokenRequest request)
        {
            var token = _authenticationService.GetAuthenticationToken(request.Username, request.Password);

            if (token is null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(new TokenResponse(token, DateTime.UtcNow.AddHours(1)));
        }
    }
}
