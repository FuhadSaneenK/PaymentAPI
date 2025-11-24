using Moq;
using PaymentAPI.Application.Handlers.Accounts;
using PaymentAPI.Application.Queries.Accounts;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Accounts;

public class GetAccountsByMerchantIdQueryHandlerTests
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task Should_ReturnNotFound_When_Merchant_NotFound()
    {
        // Arrange
        var merchantRepo = MerchantRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        merchantRepo.Setup(x => x.GetByIdAsync(20, _ct))
                    .ReturnsAsync((Merchant)null);

        var handler = new GetAccountsByMerchantIdQueryHandler(merchantRepo.Object, accountRepo.Object, LoggerMock.Create<GetAccountsByMerchantIdQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetAccountsByMerchantIdQuery(20), _ct);

        // Assert
        result.Status.ShouldBe(404);
        result.Message.ShouldBe("Merchant not found");
    }


    [Fact]
    public async Task Should_ReturnPagedAccounts_When_Merchant_Exists()
    {
        // Arrange
        var merchantRepo = MerchantRepositoryMock.Get();
        var accountRepo = AccountRepositoryMock.Get();

        var merchantId = 1;

        merchantRepo.Setup(x => x.GetByIdAsync(merchantId, _ct))
                    .ReturnsAsync(MerchantMock.GetMerchant(merchantId));

        var accounts = new List<Account>
        {
            AccountMock.GetAccount(1, merchantId, "Holder 1", 1000),
            AccountMock.GetAccount(2, merchantId, "Holder 2", 2000)
        };

        // Updated to use GetByMerchantIdPagedAsync instead of GetByMerchantIdAsync
        accountRepo.Setup(x => x.GetByMerchantIdPagedAsync(merchantId, null, 1, 20, _ct))
                   .ReturnsAsync(new PagedResult<Account>
                   {
                       Items = accounts,
                       TotalCount = 2,
                       PageNumber = 1,
                       PageSize = 20
                   });

        var handler = new GetAccountsByMerchantIdQueryHandler(merchantRepo.Object, accountRepo.Object, LoggerMock.Create<GetAccountsByMerchantIdQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetAccountsByMerchantIdQuery(merchantId), _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.Items.Count.ShouldBe(2);
        result.Data.TotalCount.ShouldBe(2);
        result.Data.Items[0].HolderName.ShouldBe("Holder 1");
        result.Data.Items[1].Balance.ShouldBe(2000);
    }
}

