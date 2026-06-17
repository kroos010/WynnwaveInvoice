using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Application.Invoices;

public interface IInvoiceRepository
{
    /// <summary>Laadt de volledige aggregate (incl. regels).</summary>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
 
    void Add(Invoice invoice);
}
