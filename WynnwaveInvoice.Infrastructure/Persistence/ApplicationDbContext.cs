using Microsoft.EntityFrameworkCore;
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
        base.OnModelCreating(modelBuilder);
    }
}
