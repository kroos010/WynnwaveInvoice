using Microsoft.EntityFrameworkCore;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Infrastructure.Persistence;

namespace WynnwaveInvoice.Infrastructure.Invoices;

/// <summary>
/// Levert het volgende factuurnummer per jaar en type, bijv. "VF-2026-0001"
/// (verkoop) of "IF-2026-0001" (inkoop).
///
/// LET OP: deze telling is niet race-safe. Voeg in productie een unieke index
/// op InvoiceNumber toe (staat al in de configuratie) en gebruik bij voorkeur
/// een Postgres-sequence per jaar, of vang de unique-violation op en probeer
/// opnieuw.
/// </summary>
public sealed class InvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private readonly ApplicationDbContext _dbContext;

    public InvoiceNumberGenerator(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<string> NextAsync(InvoiceType type, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        var year = invoiceDate.Year;
        var prefix = type == InvoiceType.Sales ? "VF" : "IF";

        var from = new DateOnly(year, 1, 1);
        var to = new DateOnly(year, 12, 31);

        var count = await _dbContext.Invoices
            .CountAsync(i => i.InvoiceType == type && i.InvoiceDate >= from && i.InvoiceDate <= to, cancellationToken);

        return $"{prefix}-{year}-{(count + 1):D4}";
    }
}
