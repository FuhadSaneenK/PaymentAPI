namespace PaymentAPI.Application.Abstractions.Services
{
    /// <summary>
    /// Service interface for JWT token generation and management.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token for authenticated users.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="role">The role assigned to the user (Admin or User).</param>
        /// <param name="merchantId">Optional merchant ID for non-admin users.</param>
        /// <returns>A JWT token string.</returns>
        string GenerateToken(int userId, string username, string role, int? merchantId = null);
    }
}
