using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Entities;


namespace PaymentAPI.Infrastructure.Persistance.Configurations
{
    public class PaymentMethodConfiguration: IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.MethodName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Provider)
                .HasMaxLength(100);

            builder.HasMany(p => p.Transactions)
                .WithOne(t => t.PaymentMethod)
                .HasForeignKey(t => t.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
