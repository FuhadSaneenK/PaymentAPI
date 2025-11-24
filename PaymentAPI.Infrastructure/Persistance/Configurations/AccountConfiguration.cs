using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Entities;


namespace PaymentAPI.Infrastructure.Persistance.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.HolderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Balance)
            .HasColumnType("numeric(18,2)");

        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
