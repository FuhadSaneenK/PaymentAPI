using Moq;
using PaymentAPI.Application.Handlers.Transactions;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Transactions;

public class MakeRefundCommandHandlerTests
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task Should_ReturnNotFound_When_Account_DoesNotExist()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        accountRepo.Setup(x => x.GetByIdAsync(999, _ct))
                   .ReturnsAsync((Account)null);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 999, 
                Amount = 100, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(404);
        result.Message.ShouldBe("Account not found");
    }

    [Fact]
    public async Task Should_ReturnFail_When_OriginalPayment_NotFound()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync((Transaction)null);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 100, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Original payment not found for this reference number");
    }

    [Fact]
    public async Task Should_ReturnFail_When_OriginalTransaction_IsNotPayment()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
        var originalTransaction = TransactionMock.GetTransaction(1, 100, "Refund", 1, 1, "REF001");

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync(originalTransaction);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 100, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Original payment not found for this reference number");
    }

    [Fact]
    public async Task Should_ReturnFail_When_RefundAmount_ExceedsOriginalAmount()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
        var originalPayment = TransactionMock.GetTransaction(1, 100, "Payment", 1, 1, "REF001");

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync(originalPayment);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 200, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Refund amount cannot exceed original payment amount");
    }

    [Fact]
    public async Task Should_ReturnFail_When_Refund_AlreadyExists()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
        var originalPayment = TransactionMock.GetTransaction(1, 100, "Payment", 1, 1, "REF001");
        var existingRefund = TransactionMock.GetTransaction(2, 100, "Refund", 1, 1, "REF001-REF");

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync(originalPayment);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001-REF", _ct))
                       .ReturnsAsync(existingRefund);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 100, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Refund already processed for this reference number");
    }

    [Fact]
    public async Task Should_ReturnFail_When_ReferenceNo_BelongsToDifferentAccount()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
        var originalPayment = TransactionMock.GetTransaction(1, 100, "Payment", 2, 1, "REF001"); // Different account

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync(originalPayment);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 100, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Reference number does not belong to this account");
    }

    [Fact]
    public async Task Should_CreateRefund_When_AllValidations_Pass()
    {
        // Arrange
        var transactionRepo = TransactionRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
        var originalPayment = TransactionMock.GetTransaction(1, 500, "Payment", 1, 1, "REF001");

        accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                   .ReturnsAsync(account);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                       .ReturnsAsync(originalPayment);

        transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001-REF", _ct))
                       .ReturnsAsync((Transaction)null);

        transactionRepo.Setup(x => x.AddAsync(It.IsAny<Transaction>(), _ct))
                       .Callback<Transaction, CancellationToken>((t, ct) => t.Id = 10)
                       .Returns(Task.CompletedTask);

        accountRepo.Setup(x => x.SaveChangesAsync(_ct))
                   .ReturnsAsync(1);

        var handler = new MakeRefundCommandHandler(transactionRepo.Object, accountRepo.Object, LoggerMock.Create<MakeRefundCommandHandler>());

        // Act
        var result = await handler.Handle(
            new MakeRefundCommand 
            { 
                AccountId = 1, 
                Amount = 300, 
                ReferenceNo = "REF001" 
            }, 
            _ct);

        // Assert
        result.Status.ShouldBe(201);
        result.Message.ShouldBe("Refund processed successfully");
        result.Data.ShouldNotBeNull();
        result.Data.Amount.ShouldBe(300);
        result.Data.Type.ShouldBe("Refund");
        result.Data.Status.ShouldBe("Completed");
        result.Data.ReferenceNo.ShouldBe("REF001-REF");
        result.Data.AccountId.ShouldBe(1);

        // Verify account balance was updated
        account.Balance.ShouldBe(700);

        // Verify repository methods were called
        transactionRepo.Verify(x => x.AddAsync(It.Is<Transaction>(t => 
            t.Amount == 300 && 
            t.Type == "Refund" && 
            t.ReferenceNumber == "REF001-REF"), _ct), Times.Once);
        accountRepo.Verify(x => x.SaveChangesAsync(_ct), Times.Once);
    }
}

