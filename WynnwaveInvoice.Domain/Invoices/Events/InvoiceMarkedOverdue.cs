using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Invoices.Events;

public sealed record InvoiceMarkedOverdue(Guid InvoiceId, string InvoiceNumber, DateOnly DueDate) : DomainEvent;
