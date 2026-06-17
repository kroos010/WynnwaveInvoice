using Microsoft.EntityFrameworkCore;
using WynnwaveInvoice.Application.Invoices;
using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _db;
 
    public InvoiceRepository(ApplicationDbContext db) => _db = db;
 
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Invoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<InvoiceListItem>> GetAllAsync(CancellationToken cancellationToken = default)
        => await (
            from inv in _db.Invoices
            join rel in _db.Relations on inv.RelationId equals rel.Id
            orderby inv.InvoiceDate descending, inv.InvoiceNumber descending
            select new InvoiceListItem(
                inv.Id,
                inv.InvoiceNumber,
                rel.Name,
                inv.InvoiceDate,
                inv.DueDate,
                inv.Lines.Sum(l => l.Quantity * l.UnitPrice * (1 + l.VATPercentage / 100m)),
                inv.Status)
        ).ToListAsync(cancellationToken);

    public void Add(Invoice invoice) => _db.Invoices.Add(invoice);
}
