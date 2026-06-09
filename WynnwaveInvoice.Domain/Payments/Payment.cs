using WynnwaveInvoice.Domain.Common;
using WynnwaveInvoice.Domain.Payments.Events;

namespace WynnwaveInvoice.Domain.Payments;

public sealed class Payment : AggregateRoot, IAuditable
{
    public Guid InvoiceId { get; private set; }
    public string? MolliePaymentId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public string? CheckoutUrl { get; private set; }
    public string? WebhookUrl { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Payment() { } // EF

    private Payment(Guid invoiceId, PaymentMethod method, decimal amount)
    {
        InvoiceId = invoiceId;
        Method = method;
        Amount = amount;
        Status = PaymentStatus.Open;
    }

    public static Payment Create(Guid invoiceId, PaymentMethod method, decimal amount)
    {
        if (amount <= 0) throw new DomainException("Het betaalbedrag moet groter zijn dan nul.");

        var payment = new Payment(invoiceId, method, amount);
        payment.Raise(new PaymentCreated(payment.Id, invoiceId, amount));
        return payment;
    }

    /// <summary>Regel 5: gegevens van Mollie zijn bevestigd; CheckoutUrl bewaren voor opnieuw versturen.</summary>
    public void ConfirmCreation(string molliePaymentId, string checkoutUrl, string webhookUrl, DateTime? expiresAt)
    {
        if (Status != PaymentStatus.Open)
            throw new DomainException("Alleen een nieuwe betaling kan worden bevestigd.");
        if (string.IsNullOrWhiteSpace(molliePaymentId))
            throw new DomainException("MolliePaymentId is verplicht.");

        MolliePaymentId = molliePaymentId;
        CheckoutUrl = checkoutUrl;
        WebhookUrl = webhookUrl;
        ExpiresAt = expiresAt;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsPaid(DateTime paidAt)
    {
        if (Status == PaymentStatus.Paid) return;
        if (Status is PaymentStatus.Failed or PaymentStatus.Expired or PaymentStatus.Canceled)
            throw new DomainException("Een afgeronde of mislukte betaling kan niet meer als betaald worden gemarkeerd.");

        Status = PaymentStatus.Paid;
        PaidAt = paidAt;
        Raise(new PaymentReceived(Id, InvoiceId, Amount, paidAt));
    }

    public void MarkAsFailed() => MoveToTerminal(PaymentStatus.Failed);
    public void MarkAsExpired() => MoveToTerminal(PaymentStatus.Expired);
    public void Cancel() => MoveToTerminal(PaymentStatus.Canceled);

    private void MoveToTerminal(PaymentStatus target)
    {
        if (Status is PaymentStatus.Paid)
            throw new DomainException("Een voldane betaling kan niet meer van status veranderen.");
        if (Status is PaymentStatus.Open or PaymentStatus.Pending)
            Status = target;
    }
}
