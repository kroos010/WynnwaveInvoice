using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Invoices.Events;

public sealed record InvoiceSent(Guid InvoiceId, string InvoiceNumber) : DomainEvent;
