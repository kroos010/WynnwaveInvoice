using WynnwaveInvoice.Domain.Common;
using WynnwaveInvoice.Domain.Invoices.Events;
using WynnwaveInvoice.Domain.Relations;
using WynnwaveInvoice.Domain.Shared.ValueObjects;

namespace WynnwaveInvoice.Domain.Invoices;

public sealed class Invoice : AggregateRoot, IAuditable
{
    private readonly List<InvoiceLine> _lines = new();
    public IReadOnlyCollection<InvoiceLine> Lines => _lines.AsReadOnly();

    public Guid RelationId { get; private set; }
    public Guid? ContactPersonId { get; private set; }
    public string InvoiceNumber { get; private set; }
    public InvoiceType InvoiceType { get; private set; }
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Invoice(Guid relationId, Guid? contactPersonId, string invoiceNumber, InvoiceType invoiceType, DateOnly invoiceDate, DateOnly dueDate, string? notes) : this()
    {
        RelationId = relationId;
        ContactPersonId = contactPersonId;
        InvoiceNumber = invoiceNumber;
        InvoiceType = invoiceType;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Notes = notes;
        Status = InvoiceStatus.Draft;
    }

    private Invoice() { InvoiceNumber = null!; }

    public static Invoice Create(Guid relationId, RelationType relationType, InvoiceType invoiceType,
                                 string invoiceNumber, DateOnly invoiceDate, DateOnly dueDate,
                                 Guid? contactPersonId = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new DomainException("Factuurnummer is verplicht.");
        if (dueDate < invoiceDate) throw new DomainException("De vervaldatum mag niet vóór de factuurdatum liggen.");

        var allowed = relationType switch
        {
            RelationType.Both => true,
            RelationType.Debtor => invoiceType == InvoiceType.Sales,
            RelationType.Creditor => invoiceType == InvoiceType.Purchase,
            _ => false
        };

        if (!allowed)
            throw new DomainException("Dit factuurtype is niet toegestaan voor deze relatie.");

        var invoice = new Invoice(relationId, contactPersonId, invoiceNumber, invoiceType, invoiceDate, dueDate, notes);
        invoice.Raise(new InvoiceCreated(invoice.Id, relationId, invoiceNumber, invoiceType));
        return invoice;
    }

    public InvoiceLine AddLine(string description, decimal quantity, decimal unitPrice, decimal vatPercentage)
    {
        EnsureEditable();
        var line = InvoiceLine.Create(Id, description, quantity, unitPrice, vatPercentage, _lines.Count);
        _lines.Add(line);
        return line;
    }

    public void RemoveLine(Guid lineId)
    {
        EnsureEditable();
        var line = _lines.FirstOrDefault(l => l.Id == lineId) ?? throw new DomainException("Factuurregel niet gevonden.");

        _lines.Remove(line);
        Reindex();
    }

    public void Send()
    {
        EnsureEditable();
        if (_lines.Count == 0) throw new DomainException("Een factuur moet minimaal één regel bevatten voordat deze verzonden kan worden.");

        Status = InvoiceStatus.Sent;
        Raise(new InvoiceSent(Id, InvoiceNumber));
    }

    public void MarkAsPaid()
    {
        if (Status == InvoiceStatus.Paid) return;
        if (Status == InvoiceStatus.Draft)
            throw new DomainException("Een conceptfactuur kan niet als betaald worden gemarkeerd.");

        Status = InvoiceStatus.Paid;
        Raise(new InvoicePaid(Id, InvoiceNumber));
    }

    public void MarkOverdueIfDue(DateOnly today)
    {
        if (Status == InvoiceStatus.Sent && today > DueDate)
        {
            Status = InvoiceStatus.Overdue;
            Raise(new InvoiceMarkedOverdue(Id, InvoiceNumber, DueDate));
        }
    }

    public Money Subtotal => _lines.Aggregate(Money.Zero(), (sum, l) => sum + l.Subtotal);
    public Money VatTotal => _lines.Aggregate(Money.Zero(), (sum, l) => sum + l.VatAmount);
    public Money Total => Subtotal + VatTotal;


    private void EnsureEditable()
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Alleen conceptfacturen kunnen worden bewerkt.");
    }

    private void Reindex()
    {
        for (var i = 0; i < _lines.Count; i++)
            _lines[i].SetSortOrder(i);
    }
}

