using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Invoices.Events;

public sealed record InvoiceCreated(Guid InvoiceId, Guid RelationId, string InvoiceNumber, InvoiceType InvoiceType) : DomainEvent;
