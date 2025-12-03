using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Infrastructure.Persistance.Configurations;

/// <summary>
/// Configuration for the <see cref="RefundRequest"/> entity.
/// </summary>
public class RefundRequestConfiguration : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.ToTable("RefundRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.OriginalPaymentReference)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Pending");

        builder.Property(r => r.RequestDate)
            .IsRequired();

        builder.Property(r => r.AdminComments)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(r => r.Account)
            .WithMany()
            .HasForeignKey(r => r.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewedByUser)
            .WithMany()
            .HasForeignKey(r => r.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RefundTransaction)
            .WithMany()
            .HasForeignKey(r => r.RefundTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.AccountId);
        builder.HasIndex(r => r.OriginalPaymentReference);
    }
}
