using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Shared.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "EUR")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Valuta is verplicht.");

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "EUR") => new(0m, currency);

    public static Money operator +(Money left, Money right) =>
        left.Currency == right.Currency
            ? new Money(left.Amount + right.Amount, left.Currency)
            : throw new DomainException("Bedragen met verschillende valuta kunnen niet worden opgeteld.");

    public static Money operator *(Money money, decimal factor) =>
        new(money.Amount * factor, money.Currency);

    public override string ToString() => $"{Currency} {Amount:0.00}";
}
