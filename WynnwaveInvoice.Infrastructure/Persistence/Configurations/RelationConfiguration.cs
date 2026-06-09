using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WynnwaveInvoice.Domain.Relations;

namespace WynnwaveInvoice.Infrastructure.Persistence.Configurations;

public sealed class RelationConfiguration : IEntityTypeConfiguration<Relation>
{
    public void Configure(EntityTypeBuilder<Relation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.VATNumber).HasMaxLength(50);
        builder.Property(r => r.ChamberOfCommerceNumber).HasMaxLength(50);
        builder.Property(r => r.Email).HasMaxLength(256);
        builder.Property(r => r.Phone).HasMaxLength(50);
        builder.Property(r => r.IBAN).HasMaxLength(34);
        builder.Property(r => r.Notes).HasMaxLength(2000);

        // Address als owned value object, platgeslagen naar kolommen op dezelfde tabel.
        builder.OwnsOne(r => r.Address, a =>
        {
            a.Property(p => p.StreetName).HasColumnName("street_name").HasMaxLength(200).IsRequired();
            a.Property(p => p.HouseNumber).HasColumnName("house_number").HasMaxLength(20).IsRequired();
            a.Property(p => p.HouseNumberAddition).HasColumnName("house_number_addition").HasMaxLength(20);
            a.Property(p => p.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
            a.Property(p => p.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            a.Property(p => p.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
        });
        builder.Navigation(r => r.Address).IsRequired();

        // ContactPersons via backing field (_contactPersons).
        builder.HasMany(r => r.ContactPersons)
               .WithOne()
               .HasForeignKey(c => c.RelationId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.ContactPersons).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(r => r.DomainEvents);
    }
}

