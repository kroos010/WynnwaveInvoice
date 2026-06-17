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
 
    public void Add(Invoice invoice) => _db.Invoices.Add(invoice);
}
