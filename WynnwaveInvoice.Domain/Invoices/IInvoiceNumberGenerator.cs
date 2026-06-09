namespace WynnwaveInvoice.Domain.Invoices;

public interface IInvoiceNumberGenerator
{
    Task<string> NextAsync(InvoiceType type, DateOnly invoiceDate, CancellationToken cancellationToken = default);
}
