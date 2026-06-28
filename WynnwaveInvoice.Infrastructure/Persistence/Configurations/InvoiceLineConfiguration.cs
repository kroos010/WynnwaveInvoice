using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Infrastructure.Persistence.Configurations;

public sealed class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Description).HasMaxLength(500).IsRequired();
        builder.Property(l => l.Quantity).HasPrecision(18, 3);
        builder.Property(l => l.UnitPrice).HasPrecision(18, 2);
        builder.Property(l => l.VATPercentage).HasPrecision(5, 2);
        builder.Property(l => l.PeriodStart).IsRequired(false);
        builder.Property(l => l.PeriodEnd).IsRequired(false);

        builder.Ignore(l => l.Subtotal);
        builder.Ignore(l => l.VatAmount);
        builder.Ignore(l => l.Total);
    }
}
