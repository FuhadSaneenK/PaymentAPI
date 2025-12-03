using Moq;
using PaymentAPI.Application.Handlers.Merchants;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Merchants;

/// <summary>
/// Unit tests for <see cref="GetAllMerchantsQueryHandler"/>.
/// </summary>
public class GetAllMerchantsQueryHandlerTests
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task Should_ReturnEmptyList_When_NoMerchants_Exist()
    {
        // Arrange
        var merchantRepo = MerchantRepositoryMock.Get();

        merchantRepo.Setup(x => x.GetAllAsync(_ct))
                    .ReturnsAsync(new List<Merchant>());

        var handler = new GetAllMerchantsQueryHandler(
            merchantRepo.Object, 
            LoggerMock.Create<GetAllMerchantsQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetAllMerchantsQuery(), 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnAllMerchants_When_Merchants_Exist()
    {
        // Arrange
        var merchantRepo = MerchantRepositoryMock.Get();

        var merchants = new List<Merchant>
        {
            MerchantMock.GetMerchant(1, "Merchant One", "merchant1@test.com"),
            MerchantMock.GetMerchant(2, "Merchant Two", "merchant2@test.com"),
            MerchantMock.GetMerchant(3, "Merchant Three", "merchant3@test.com")
        };

        merchantRepo.Setup(x => x.GetAllAsync(_ct))
                    .ReturnsAsync(merchants);

        var handler = new GetAllMerchantsQueryHandler(
            merchantRepo.Object, 
            LoggerMock.Create<GetAllMerchantsQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetAllMerchantsQuery(), 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(3);
        
        result.Data[0].Id.ShouldBe(1);
        result.Data[0].Name.ShouldBe("Merchant One");
        result.Data[0].Email.ShouldBe("merchant1@test.com");
        
        result.Data[1].Id.ShouldBe(2);
        result.Data[1].Name.ShouldBe("Merchant Two");
        result.Data[1].Email.ShouldBe("merchant2@test.com");
        
        result.Data[2].Id.ShouldBe(3);
        result.Data[2].Name.ShouldBe("Merchant Three");
        result.Data[2].Email.ShouldBe("merchant3@test.com");
    }

    [Fact]
    public async Task Should_ReturnSingleMerchant_When_Only_OneMerchant_Exists()
    {
        // Arrange
        var merchantRepo = MerchantRepositoryMock.Get();

        var merchants = new List<Merchant>
        {
            MerchantMock.GetMerchant(1, "Single Merchant", "single@test.com")
        };

        merchantRepo.Setup(x => x.GetAllAsync(_ct))
                    .ReturnsAsync(merchants);

        var handler = new GetAllMerchantsQueryHandler(
            merchantRepo.Object, 
            LoggerMock.Create<GetAllMerchantsQueryHandler>());

        // Act
        var result = await handler.Handle(
            new GetAllMerchantsQuery(), 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
        result.Data[0].Id.ShouldBe(1);
        result.Data[0].Name.ShouldBe("Single Merchant");
        result.Data[0].Email.ShouldBe("single@test.com");
    }
}
