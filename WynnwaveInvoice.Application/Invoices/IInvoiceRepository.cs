using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Application.Invoices;

public interface IInvoiceRepository
{
    /// <summary>Laadt de volledige aggregate (incl. regels).</summary>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lijstweergave gesorteerd op factuurdatum aflopend.</summary>
    Task<IReadOnlyList<InvoiceListItem>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(Invoice invoice);
}

public sealed record InvoiceListItem(
    Guid Id,
    string InvoiceNumber,
    string RelationName,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    decimal Total,
    InvoiceStatus Status);
