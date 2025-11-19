using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;


namespace PaymentAPI.Application.Commands.Accounts
{
    /// <summary>
    /// Command to create a new account for a specific merchant.
    /// Validates merchant existence before creating the account.
    /// </summary>
    public class CreateAccountCommand:IRequest<ApiResponse<AccountDto>>
    {
        /// <summary>
        /// Gets or sets the name of the account holder.
        /// </summary>
        public string HolderName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the initial balance for the account.
        /// </summary>
        public decimal Balance { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the merchant who owns this account.
        /// </summary>
        public int MerchantId { get; set; }
    }
}
