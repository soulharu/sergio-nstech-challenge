namespace Orders.API.Contracts.Authentication
{
    public record TokenResponse(string Token, DateTime ExpiresAt)
    {
    }
}
