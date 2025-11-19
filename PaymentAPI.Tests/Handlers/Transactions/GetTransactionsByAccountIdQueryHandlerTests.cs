using Moq;
using PaymentAPI.Application.Handlers.Transactions;
using PaymentAPI.Application.Queries.Transactions;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Transactions
{
    public class GetTransactionsByAccountIdQueryHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnNotFound_When_Account_DoesNotExist()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            accountRepo.Setup(x => x.GetByIdAsync(999, _ct))
                       .ReturnsAsync((Account)null);

            var handler = new GetTransactionsByAccountIdQueryHandler(
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetTransactionsByAccountIdQuery(999), 
                _ct);

            // Assert
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("Account not found");
        }

        [Fact]
        public async Task Should_ReturnEmptyList_When_Account_HasNoTransactions()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);

            accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                       .ReturnsAsync(account);

            transactionRepo.Setup(x => x.GetByAccountIdAsync(1, _ct))
                           .ReturnsAsync(new List<Transaction>());

            var handler = new GetTransactionsByAccountIdQueryHandler(
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetTransactionsByAccountIdQuery(1), 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_ReturnTransactions_When_Account_HasTransactions()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);

            var transactions = new List<Transaction>
            {
                TransactionMock.GetTransaction(1, 500, "Payment", 1, 1, "REF001"),
                TransactionMock.GetTransaction(2, 200, "Refund", 1, 1, "REF002"),
                TransactionMock.GetTransaction(3, 300, "Payment", 1, 2, "REF003")
            };

            accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                       .ReturnsAsync(account);

            transactionRepo.Setup(x => x.GetByAccountIdAsync(1, _ct))
                           .ReturnsAsync(transactions);

            var handler = new GetTransactionsByAccountIdQueryHandler(
                accountRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetTransactionsByAccountIdQuery(1), 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(3);
            result.Data[0].Amount.ShouldBe(500);
            result.Data[0].Type.ShouldBe("Payment");
            result.Data[1].Amount.ShouldBe(200);
            result.Data[1].Type.ShouldBe("Refund");
            result.Data[2].Amount.ShouldBe(300);
            result.Data[2].PaymentMethodId.ShouldBe(2);
        }
    }
}
