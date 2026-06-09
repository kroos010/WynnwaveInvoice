using WynnwaveInvoice.Domain.Common;
using WynnwaveInvoice.Domain.Shared.ValueObjects;

namespace WynnwaveInvoice.Domain.Invoices;

public sealed class InvoiceLine : Entity
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal VATPercentage { get; private set; }
    public int SortOrder { get; private set; }


    private InvoiceLine(Guid invoiceId, string description, decimal quantity,
                        decimal unitPrice, decimal vatPercentage, int sortOrder)
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        VATPercentage = vatPercentage;
        SortOrder = sortOrder;
    }

    internal static InvoiceLine Create(Guid invoiceId, string description, decimal quantity,
                                       decimal unitPrice, decimal vatPercentage, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Omschrijving van de factuurregel is verplicht.");
        if (quantity <= 0) throw new DomainException("Aantal moet groter zijn dan nul.");
        if (unitPrice < 0) throw new DomainException("Stukprijs mag niet negatief zijn.");
        if (vatPercentage < 0) throw new DomainException("Btw-percentage mag niet negatief zijn.");

        return new InvoiceLine(invoiceId, description, quantity, unitPrice, vatPercentage, sortOrder);
    }

    internal void SetSortOrder(int sortOrder) => SortOrder = sortOrder;

    // Berekend, nooit opgeslagen:
    public Money Subtotal => new(Quantity * UnitPrice);
    public Money VatAmount => new(Quantity * UnitPrice * (VATPercentage / 100m));
    public Money Total => Subtotal + VatAmount;

    private InvoiceLine() { Description = null!; } // EF
}
