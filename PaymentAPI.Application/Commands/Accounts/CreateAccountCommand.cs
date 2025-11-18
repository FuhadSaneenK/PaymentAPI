using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;


namespace PaymentAPI.Application.Commands.Accounts
{
    public class CreateAccountCommand:IRequest<ApiResponse<AccountDto>>
    {
        public string HolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int MerchantId { get; set; }
    }
}
