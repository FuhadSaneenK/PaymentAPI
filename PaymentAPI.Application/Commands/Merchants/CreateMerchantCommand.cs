using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;


namespace PaymentAPI.Application.Commands.Merchants
{
    public class CreateMerchantCommand : IRequest<ApiResponse<MerchantDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
    }
}
