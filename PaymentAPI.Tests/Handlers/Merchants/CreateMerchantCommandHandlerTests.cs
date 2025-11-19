using Moq;
using PaymentAPI.Application.Handlers.Merchants;
using PaymentAPI.Application.Commands.Merchants;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Merchants
{
    public class CreateMerchantCommandHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnFail_When_Email_AlreadyExists()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();

            var existingMerchant = MerchantMock.GetMerchant(1, "Existing Merchant", "existing@test.com");

            merchantRepo.Setup(x => x.GetByEmailAsync("existing@test.com", _ct))
                        .ReturnsAsync(existingMerchant);

            var handler = new CreateMerchantCommandHandler(merchantRepo.Object);

            // Act
            var result = await handler.Handle(
                new CreateMerchantCommand { Name = "New Merchant", Email = "existing@test.com" }, 
                _ct);

            // Assert
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Merchant with this email already exists.");
        }

        [Fact]
        public async Task Should_CreateMerchant_When_Email_IsUnique()
        {
            // Arrange
            var merchantRepo = MerchantRepositoryMock.Get();

            merchantRepo.Setup(x => x.GetByEmailAsync("new@test.com", _ct))
                        .ReturnsAsync((Merchant)null);

            merchantRepo.Setup(x => x.AddAsync(It.IsAny<Merchant>(), _ct))
                        .Callback<Merchant, CancellationToken>((m, ct) => m.Id = 5)
                        .Returns(Task.CompletedTask);

            merchantRepo.Setup(x => x.SaveChangesAsync(_ct))
                        .ReturnsAsync(1);

            var handler = new CreateMerchantCommandHandler(merchantRepo.Object);

            // Act
            var result = await handler.Handle(
                new CreateMerchantCommand { Name = "New Merchant", Email = "new@test.com" }, 
                _ct);

            // Assert
            result.Status.ShouldBe(201);
            result.Message.ShouldBe("Merchant created successfully");
            result.Data.ShouldNotBeNull();
            result.Data.Name.ShouldBe("New Merchant");
            result.Data.Email.ShouldBe("new@test.com");

            // Verify repository methods were called
            merchantRepo.Verify(x => x.AddAsync(It.Is<Merchant>(m => 
                m.Name == "New Merchant" && 
                m.Email == "new@test.com"), _ct), Times.Once);
            merchantRepo.Verify(x => x.SaveChangesAsync(_ct), Times.Once);
        }
    }
}
