using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Entities;


namespace PaymentAPI.Infrastructure.Persistance.Configurations
{
    public class MerchantConfiguration:IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(150);

            builder.HasMany(m => m.Accounts)
            .WithOne(a => a.Merchant)
            .HasForeignKey(a => a.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
