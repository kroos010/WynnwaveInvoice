using WynnwaveInvoice.Application.Common;
using WynnwaveInvoice.Application.Relations;
using WynnwaveInvoice.Domain.Invoices;

namespace WynnwaveInvoice.Application.Invoices;

public sealed class InvoiceService
{
    private readonly IInvoiceRepository _invoices;
    private readonly IRelationRepository _relations;
    private readonly IInvoiceNumberGenerator _numbers;
    private readonly IUnitOfWork _unitOfWork;
 
    public InvoiceService( IInvoiceRepository invoices, IRelationRepository relations,  IInvoiceNumberGenerator numbers, IUnitOfWork unitOfWork)
    {
        _invoices = invoices;
        _relations = relations;
        _numbers = numbers;
        _unitOfWork = unitOfWork;
    }
 
    public Task<IReadOnlyList<InvoiceListItem>> GetAllAsync(CancellationToken ct = default)
        => _invoices.GetAllAsync(ct);

    public async Task<Guid> CreateDraftAsync(CreateDraftInvoiceRequest request, CancellationToken ct = default)
    {
        var relation = await _relations.GetByIdAsync(request.RelationId, ct)
                       ?? throw new InvalidOperationException("Relatie niet gevonden.");
 
        var number = await _numbers.NextAsync(request.InvoiceType, request.InvoiceDate, ct);
 
        var invoice = Invoice.Create(
            relation.Id,
            relation.Type,                 // regel-7-controle zit in de aggregate
            request.InvoiceType,
            number,
            request.InvoiceDate,
            request.InvoiceDate.AddDays(request.PaymentTermDays));
 
        foreach (var line in request.Lines)
            invoice.AddLine(line.Description, line.Quantity, line.UnitPrice, line.VatPercentage, line.PeriodStart, line.PeriodEnd);
 
        _invoices.Add(invoice);
        await _unitOfWork.SaveChangesAsync(ct);
        return invoice.Id;
    }
}
 
public sealed record CreateDraftInvoiceRequest(
    Guid RelationId,
    InvoiceType InvoiceType,
    DateOnly InvoiceDate,
    int PaymentTermDays,
    IReadOnlyList<CreateInvoiceLine> Lines);
 
public sealed record CreateInvoiceLine(
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal VatPercentage,
    DateOnly? PeriodStart = null,
    DateOnly? PeriodEnd = null);
