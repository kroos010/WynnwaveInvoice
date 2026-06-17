using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Domain.Payments;
using WynnwaveInvoice.Domain.Relations;

namespace WynnwaveInvoice.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Relation> Relations => Set<Relation>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Het domein genereert alle Guid-id's zelf (Entity: Id = Guid.NewGuid()).
        // Zonder dit ziet EF een nieuw kind met gevulde key aan voor "bestaat al" -> UPDATE i.p.v. INSERT.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!entityType.IsOwned())
            {
            }
            
            if (entityType.IsOwned()) continue; // Address e.d. overslaan
            var id = entityType.FindProperty("Id");
            if (id is not null && id.ClrType == typeof(Guid))
                id.ValueGenerated = ValueGenerated.Never;
        }
        
        base.OnModelCreating(modelBuilder);
    }
}
