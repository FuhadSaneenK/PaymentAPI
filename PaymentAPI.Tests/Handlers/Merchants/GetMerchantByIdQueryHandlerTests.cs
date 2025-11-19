using Moq;
using PaymentAPI.Application.Handlers.Merchants;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Merchants
{
    public class GetMerchantByIdQueryHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnNotFound_When_Merchant_DoesNotExist()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();

            merchantRepo.Setup(x => x.GetByIdAsync(999, _ct))
                        .ReturnsAsync((Merchant)null);

            var handler = new GetMerchantByIdQueryHandler(merchantRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetMerchantByIdQuery(999), 
                _ct);

            // Assert
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("Merchant not found");
        }

        [Fact]
        public async Task Should_ReturnMerchant_When_Merchant_Exists()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();

            var merchant = MerchantMock.GetMerchant(1, "Test Merchant", "test@merchant.com");

            merchantRepo.Setup(x => x.GetByIdAsync(1, _ct))
                        .ReturnsAsync(merchant);

            var handler = new GetMerchantByIdQueryHandler(merchantRepo.Object);

            // Act
            var result = await handler.Handle(
                new GetMerchantByIdQuery(1), 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Id.ShouldBe(1);
            result.Data.Name.ShouldBe("Test Merchant");
            result.Data.Email.ShouldBe("test@merchant.com");
        }
    }
}
