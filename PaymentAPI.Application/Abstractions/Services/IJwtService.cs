namespace PaymentAPI.Application.Abstractions.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string username, string role);
    }
}
