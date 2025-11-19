using Moq;
using PaymentAPI.Application.Handlers.Transactions;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Transactions
{
    public class MakePaymentCommandHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnNotFound_When_Account_DoesNotExist()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var paymentMethodRepo = PaymentMethodRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            accountRepo.Setup(x => x.GetByIdAsync(999, _ct))
                       .ReturnsAsync((Account)null);

            var handler = new MakePaymentCommandHandler(
                accountRepo.Object,
                paymentMethodRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new MakePaymentCommand 
                { 
                    AccountId = 999, 
                    Amount = 100, 
                    PaymentMethodId = 1, 
                    ReferenceNo = "REF001" 
                }, 
                _ct);

            // Assert
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("Account not found");
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_PaymentMethod_DoesNotExist()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var paymentMethodRepo = PaymentMethodRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);

            accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                       .ReturnsAsync(account);

            paymentMethodRepo.Setup(x => x.GetByIdAsync(999, _ct))
                             .ReturnsAsync((PaymentMethod)null);

            var handler = new MakePaymentCommandHandler(
                accountRepo.Object,
                paymentMethodRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new MakePaymentCommand 
                { 
                    AccountId = 1, 
                    Amount = 100, 
                    PaymentMethodId = 999, 
                    ReferenceNo = "REF001" 
                }, 
                _ct);

            // Assert
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("Payment method not found");
        }

        [Fact]
        public async Task Should_ReturnFail_When_ReferenceNo_AlreadyExists()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var paymentMethodRepo = PaymentMethodRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
            var paymentMethod = PaymentMethodMock.GetPaymentMethod(1, "Credit Card");
            var existingTransaction = TransactionMock.GetTransaction(1, 100, "Payment", 1, 1, "REF001");

            accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                       .ReturnsAsync(account);

            paymentMethodRepo.Setup(x => x.GetByIdAsync(1, _ct))
                             .ReturnsAsync(paymentMethod);

            transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                           .ReturnsAsync(existingTransaction);

            var handler = new MakePaymentCommandHandler(
                accountRepo.Object,
                paymentMethodRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new MakePaymentCommand 
                { 
                    AccountId = 1, 
                    Amount = 100, 
                    PaymentMethodId = 1, 
                    ReferenceNo = "REF001" 
                }, 
                _ct);

            // Assert
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Reference number already exists");
        }

        [Fact]
        public async Task Should_CreatePayment_When_AllValidations_Pass()
        {
            // Arrange
            var accountRepo = AccountRepositoryMock.Get();
            var paymentMethodRepo = PaymentMethodRepositoryMock.Get();
            var transactionRepo = TransactionRepositoryMock.Get();

            var account = AccountMock.GetAccount(1, 1, "Test Holder", 1000);
            var paymentMethod = PaymentMethodMock.GetPaymentMethod(1, "Credit Card");

            accountRepo.Setup(x => x.GetByIdAsync(1, _ct))
                       .ReturnsAsync(account);

            paymentMethodRepo.Setup(x => x.GetByIdAsync(1, _ct))
                             .ReturnsAsync(paymentMethod);

            transactionRepo.Setup(x => x.GetByReferenceNoAsync("REF001", _ct))
                           .ReturnsAsync((Transaction)null);

            transactionRepo.Setup(x => x.AddAsync(It.IsAny<Transaction>(), _ct))
                           .Callback<Transaction, CancellationToken>((t, ct) => t.Id = 10)
                           .Returns(Task.CompletedTask);

            accountRepo.Setup(x => x.SaveChangesAsync(_ct))
                       .ReturnsAsync(1);

            var handler = new MakePaymentCommandHandler(
                accountRepo.Object,
                paymentMethodRepo.Object,
                transactionRepo.Object);

            // Act
            var result = await handler.Handle(
                new MakePaymentCommand 
                { 
                    AccountId = 1, 
                    Amount = 500, 
                    PaymentMethodId = 1, 
                    ReferenceNo = "REF001" 
                }, 
                _ct);

            // Assert
            result.Status.ShouldBe(201);
            result.Message.ShouldBe("Payment recorded successfully");
            result.Data.ShouldNotBeNull();
            result.Data.Amount.ShouldBe(500);
            result.Data.Type.ShouldBe("Payment");
            result.Data.Status.ShouldBe("Completed");
            result.Data.ReferenceNo.ShouldBe("REF001");
            result.Data.AccountId.ShouldBe(1);
            result.Data.PaymentMethodId.ShouldBe(1);

            // Verify account balance was updated
            account.Balance.ShouldBe(1500);

            // Verify repository methods were called
            transactionRepo.Verify(x => x.AddAsync(It.Is<Transaction>(t => 
                t.Amount == 500 && 
                t.Type == "Payment" && 
                t.ReferenceNumber == "REF001"), _ct), Times.Once);
            accountRepo.Verify(x => x.SaveChangesAsync(_ct), Times.Once);
        }
    }
}
