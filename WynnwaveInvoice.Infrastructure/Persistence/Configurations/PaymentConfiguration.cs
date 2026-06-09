using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Domain.Payments;

namespace WynnwaveInvoice.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.MolliePaymentId).HasMaxLength(100);
        builder.Property(p => p.CheckoutUrl).HasMaxLength(2000);
        builder.Property(p => p.WebhookUrl).HasMaxLength(2000);

        builder.HasIndex(p => p.MolliePaymentId).IsUnique();
        builder.HasIndex(p => p.InvoiceId);

        // FK naar Invoice voor integriteit, maar GEEN navigatie (aparte aggregate).
        builder.HasOne<Invoice>()
               .WithMany()
               .HasForeignKey(p => p.InvoiceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.DomainEvents);
    }
}
