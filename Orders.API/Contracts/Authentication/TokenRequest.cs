namespace Orders.API.Contracts.Authentication
{
    public record TokenRequest(string Username, string Password)
    {
    }
}
