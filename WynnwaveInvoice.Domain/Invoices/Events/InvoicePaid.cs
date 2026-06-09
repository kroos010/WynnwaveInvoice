using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Invoices.Events;

public sealed record InvoicePaid(Guid InvoiceId, string InvoiceNumber) : DomainEvent;
