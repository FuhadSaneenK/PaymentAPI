using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Transactions
{
    public class MakePaymentCommandHandler:IRequestHandler<MakePaymentCommand,ApiResponse<TransactionDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ITransactionRepository _transactionRepository;
        public MakePaymentCommandHandler(IAccountRepository accountRepository,IPaymentMethodRepository paymentMethodRepository,ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate Account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
                return ApiResponse<TransactionDto>.NotFound("Account not found");

            // 2. Validate Payment Method
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(request.PaymentMethodId, cancellationToken);
            if (paymentMethod == null)
                return ApiResponse<TransactionDto>.NotFound("Payment method not found");

            // 3. Reference number unique check
            var existingTxn = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo, cancellationToken);
            if (existingTxn != null)
                return ApiResponse<TransactionDto>.Fail("Reference number already exists");

            // 4. Create Transaction
            var transaction = new Transaction
            {
                Amount = request.Amount,
                AccountId = request.AccountId,
                PaymentMethodId = request.PaymentMethodId,
                ReferenceNumber = request.ReferenceNo,
                Type = "Payment",
                Status = "Completed",
                Date = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction, cancellationToken);

            // 5. Update account balance
            account.Balance += request.Amount;

            await _accountRepository.SaveChangesAsync(cancellationToken);

            // 6. Map to DTO
            var dto = new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Status = transaction.Status,
                ReferenceNo = transaction.ReferenceNumber,
                AccountId = transaction.AccountId,
                PaymentMethodId = transaction.PaymentMethodId,
                Date = transaction.Date
            };

            return ApiResponse<TransactionDto>.Created(dto, "Payment recorded successfully");
        }
    }
}
