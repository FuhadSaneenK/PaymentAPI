using Moq;
using PaymentAPI.Application.Handlers.Transactions;
using PaymentAPI.Application.Queries.Transactions;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Transactions;

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

        var handler = new GetTransactionsByAccountIdQueryHandler(accountRepo.Object, transactionRepo.Object, LoggerMock.Create<GetTransactionsByAccountIdQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetTransactionsByAccountIdQuery(999), 
            _ct);

        // Assert
        result.Status.ShouldBe(404);
        result.Message.ShouldBe("Account not found");
    }

    [Fact]
    public async Task Should_ReturnEmptyPage_When_Account_HasNoTransactions()
    {
        // Arrange
        var accountRepo = AccountRepositoryMock.Get();
        var transactionRepo = TransactionRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByAccountIdPagedAsync(1, null, null, null, null, 1, 20, _ct))
                       .ReturnsAsync(new PagedResult<Transaction>
                       {
                           Items = new List<Transaction>(),
                           TotalCount = 0,
                           PageNumber = 1,
                           PageSize = 20
                       });

        var handler = new GetTransactionsByAccountIdQueryHandler(accountRepo.Object, transactionRepo.Object, LoggerMock.Create<GetTransactionsByAccountIdQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetTransactionsByAccountIdQuery(1), 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.Items.Count.ShouldBe(0);
        result.Data.TotalCount.ShouldBe(0);
        result.Data.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task Should_ReturnPagedTransactions_When_Account_HasTransactions()
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

        transactionRepo.Setup(x => x.GetByAccountIdPagedAsync(1, null, null, null, null, 1, 20, _ct))
                       .ReturnsAsync(new PagedResult<Transaction>
                       {
                           Items = transactions,
                           TotalCount = 3,
                           PageNumber = 1,
                           PageSize = 20
                       });

        var handler = new GetTransactionsByAccountIdQueryHandler(accountRepo.Object, transactionRepo.Object, LoggerMock.Create<GetTransactionsByAccountIdQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetTransactionsByAccountIdQuery(1), 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.Items.Count.ShouldBe(3);
        result.Data.TotalCount.ShouldBe(3);
        result.Data.PageNumber.ShouldBe(1);
        result.Data.Items[0].Amount.ShouldBe(500);
        result.Data.Items[0].Type.ShouldBe("Payment");
        result.Data.Items[1].Amount.ShouldBe(200);
        result.Data.Items[1].Type.ShouldBe("Refund");
    }
}

