using Moq;
using PaymentAPI.Application.Handlers.Merchants;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Merchants
{
    public class GetMerchantSummaryQueryHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnNotFound_When_Merchant_DoesNotExist()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            merchantRepo.Setup(x => x.GetByIdAsync(999, _ct))
                        .ReturnsAsync((Merchant)null);

            var handler = new GetMerchantSummaryQueryHandler(
                merchantRepo.Object,
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetMerchantSummaryQuery(999), 
                _ct);

            // Assert
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("Merchant not found");
        }

        [Fact]
        public async Task Should_ReturnSummary_When_Merchant_HasNoAccounts()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var merchant = MerchantMock.GetMerchant(1, "Test Merchant", "test@merchant.com");

            merchantRepo.Setup(x => x.GetByIdAsync(1, _ct))
                        .ReturnsAsync(merchant);

            accountRepo.Setup(x => x.GetAllAsync(_ct))
                       .ReturnsAsync(new List<Account>());

            var handler = new GetMerchantSummaryQueryHandler(
                merchantRepo.Object,
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetMerchantSummaryQuery(1), 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.MerchantId.ShouldBe(1);
            result.Data.MerchantName.ShouldBe("Test Merchant");
            result.Data.TotalBalance.ShouldBe(0);
            result.Data.TotalTransactions.ShouldBe(0);
            result.Data.TotalHolders.ShouldBe(0);
        }

        [Fact]
        public async Task Should_ReturnSummary_When_Merchant_HasAccountsAndTransactions()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var merchantId = 1;
            var merchant = MerchantMock.GetMerchant(merchantId, "Test Merchant", "test@merchant.com");

            var accounts = new List<Account>
            {
                AccountMock.GetAccount(1, merchantId, "Holder 1", 1000),
                AccountMock.GetAccount(2, merchantId, "Holder 2", 2000)
            };

            var transactions = new List<Transaction>
            {
                TransactionMock.GetTransaction(1, 500, "Payment", 1, 1),
                TransactionMock.GetTransaction(2, 1000, "Payment", 2, 1),
                TransactionMock.GetTransaction(3, 200, "Refund", 1, 1)
            };

            merchantRepo.Setup(x => x.GetByIdAsync(merchantId, _ct))
                        .ReturnsAsync(merchant);

            accountRepo.Setup(x => x.GetAllAsync(_ct))
                       .ReturnsAsync(accounts);

            transactionRepo.Setup(x => x.GetByAccountIdAsync(1, _ct))
                           .ReturnsAsync(transactions.Where(t => t.AccountId == 1).ToList());

            transactionRepo.Setup(x => x.GetByAccountIdAsync(2, _ct))
                           .ReturnsAsync(transactions.Where(t => t.AccountId == 2).ToList());

            var handler = new GetMerchantSummaryQueryHandler(
                merchantRepo.Object,
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetMerchantSummaryQuery(merchantId), 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.MerchantId.ShouldBe(merchantId);
            result.Data.MerchantName.ShouldBe("Test Merchant");
            result.Data.Email.ShouldBe("test@merchant.com");
            result.Data.TotalBalance.ShouldBe(3000);
            result.Data.TotalHolders.ShouldBe(2);
            result.Data.TotalTransactions.ShouldBe(3);
            result.Data.TotalPayments.ShouldBe(2);
            result.Data.TotalRefunds.ShouldBe(1);
        }
    }
}
