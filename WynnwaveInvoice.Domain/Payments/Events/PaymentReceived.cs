using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Payments.Events;

public sealed record PaymentReceived(Guid PaymentId, Guid InvoiceId, decimal Amount, DateTime PaidAt) : DomainEvent;
