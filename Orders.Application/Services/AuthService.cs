using Orders.Application.Interfaces;
using Orders.Domain.Interfaces.Services;
using System.Security.Claims;
using System.Text;

namespace Orders.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly ITokenService _tokenService;

        private static readonly Dictionary<string, string> _users = new()
        {
            { "admin", "admin123" },
            { "user", "user123" }
        };

        public AuthService(ITokenService tokenService) => _tokenService = tokenService;

        public string? GetAuthenticationToken(string username, string password)
        {
            if (!_users.TryGetValue(username, out var storedPassword) || storedPassword != password)
                return null;

            return _tokenService.GenerateToken(username, username == "admin" ? "Admin" : "User");
        }
    }
}
