namespace Orders.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        string? GetAuthenticationToken(string username, string password);
    }
}
