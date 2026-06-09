using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Payments.Events;

public sealed record PaymentCreated(Guid PaymentId, Guid InvoiceId, decimal Amount) : DomainEvent;
