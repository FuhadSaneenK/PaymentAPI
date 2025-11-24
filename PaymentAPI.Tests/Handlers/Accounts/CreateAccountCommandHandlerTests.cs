using Moq;
using PaymentAPI.Application.Commands.Accounts;
using PaymentAPI.Application.Handlers.Accounts;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Accounts;


public class CreateAccountCommandHandlerTests
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task Should_ReturnNotFound_When_Merchant_Does_Not_Exist()
    {
        // Arrange
        var accountRepo = AccountRepositoryMock.Get();
        var merchantRepo = MerchantRepositoryMock.Get();
        var command = new CreateAccountCommand
        {
            HolderName = "John",
            Balance = 1000,
            MerchantId = 10
        };

        merchantRepo.Setup(x => x.GetByIdAsync(command.MerchantId, _ct))
                    .ReturnsAsync((Merchant)null);

        var handler = new CreateAccountCommandHandler(accountRepo.Object, merchantRepo.Object
        , LoggerMock.Create<CreateAccountCommandHandler>());

        // Act
        var result = await handler.Handle(command, _ct);

        // Assert
        result.Status.ShouldBe(404);
        result.Message.ShouldBe("Merchant not found");
    }


    [Fact]
    public async Task Should_CreateAccount_When_Valid()
    {
        // Arrange
        var accountRepo = AccountRepositoryMock.Get();
        var merchantRepo = MerchantRepositoryMock.Get();

        var merchant = MerchantMock.GetMerchant(1);

        var command = new CreateAccountCommand  
        {
            HolderName = "Tony Stark",
            Balance = 5000,
            MerchantId = 1
        };

        merchantRepo.Setup(x => x.GetByIdAsync(command.MerchantId, _ct))
                    .ReturnsAsync(merchant);

        accountRepo.Setup(x => x.AddAsync(It.IsAny<Account>(), _ct))
                   .Returns(Task.CompletedTask);

        accountRepo.Setup(x => x.SaveChangesAsync(_ct))
                   .ReturnsAsync(1);

        var handler = new CreateAccountCommandHandler(accountRepo.Object, merchantRepo.Object
        , LoggerMock.Create<CreateAccountCommandHandler>());

        // Act
        var result = await handler.Handle(command, _ct);

        // Assert
        result.Status.ShouldBe(201);
        result.Data.HolderName.ShouldBe(command.HolderName);
        result.Data.Balance.ShouldBe(command.Balance);
        result.Data.MerchantId.ShouldBe(command.MerchantId);
    }

}

