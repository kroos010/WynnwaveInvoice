using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        builder.Property(i => i.InvoiceType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.Notes).HasMaxLength(2000);

        builder.HasIndex(i => i.RelationId);

        // Regels via backing field (_lines).
        builder.HasMany(i => i.Lines)
               .WithOne()
               .HasForeignKey(l => l.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(i => i.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Berekende totalen worden nooit opgeslagen.
        builder.Ignore(i => i.Subtotal);
        builder.Ignore(i => i.VatTotal);
        builder.Ignore(i => i.Total);
        builder.Ignore(i => i.DomainEvents);
    }
}
