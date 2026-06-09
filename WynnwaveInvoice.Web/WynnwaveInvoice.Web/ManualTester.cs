using Microsoft.EntityFrameworkCore;
using WynnwaveInvoice.Domain.Common;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Domain.Relations;
using WynnwaveInvoice.Domain.Shared.ValueObjects;
using WynnwaveInvoice.Infrastructure.Persistence;

namespace WynnwaveInvoice.Web;

public class ManualTester
{
    private readonly ApplicationDbContext _db;
    private readonly IInvoiceNumberGenerator _numbers;

    public ManualTester(ApplicationDbContext db, IInvoiceNumberGenerator numbers)
    {
        _db = db;
        _numbers = numbers;
    }
    
    public async Task CreateInvoice()
    {
        // 1) Relatie aanmaken via de factory (nooit 'new')
        var relation = Relation.Create(
            RelationType.Debtor,
            "De Vries Architecten",
            new Address("Keizersgracht", "123", "B", "1015 CJ", "Amsterdam", "Nederland"),
            email: "info@devries.nl",
            vatNumber: "NL001234567B01");

        relation.AddContactPerson("Anna", "de Vries", email: "anna@devries.nl", isPrimaryContact: true);

        _db.Relations.Add(relation);
        await _db.SaveChangesAsync(); // RelationCreated + ContactPersonAdded worden hier verspreid

        // 2) Factuur aanmaken — nummer komt uit de generator
        var invoiceDate = DateOnly.FromDateTime(DateTime.Today);
        var invoiceNumber = await _numbers.NextAsync(InvoiceType.Sales, invoiceDate);

        var invoice = Invoice.Create(
            relationId: relation.Id,
            relationType: relation.Type,           // hierin zit de regel-7-controle
            invoiceType: InvoiceType.Sales,
            invoiceNumber: invoiceNumber,
            invoiceDate: invoiceDate,
            dueDate: invoiceDate.AddDays(30),
            contactPersonId: relation.ContactPersons.First().Id);

        invoice.AddLine("Ontwerp huisstijl", quantity: 10, unitPrice: 95m, vatPercentage: 21m);
        invoice.AddLine("Drukwerk", quantity: 1, unitPrice: 250m, vatPercentage: 9m);
        invoice.Send();                            // vereist minimaal één regel

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        // 3) Teruglezen uit de database en de BEREKENDE totalen tonen
        var reloaded = await _db.Invoices
            .Include(i => i.Lines)
            .FirstAsync(i => i.Id == invoice.Id);

        // Console.WriteLine($"Factuur {reloaded.InvoiceNumber} — status {reloaded.Status}");
        // Console.WriteLine($"  Subtotaal : {reloaded.Subtotal}");   // EUR 1200.00
        // Console.WriteLine($"  Btw       : {reloaded.VatTotal}");   // EUR 222.00
        // Console.WriteLine($"  Totaal    : {reloaded.Total}");      // EUR 1422.00

        // 4) Negatieve tests — de aggregates horen deze te WEIGEREN
        try
        {
            // Debtor mag geen inkoopfactuur ontvangen (regel 7)
            Invoice.Create(relation.Id, relation.Type, InvoiceType.Purchase,
                "TEST", invoiceDate, invoiceDate.AddDays(30));
            Console.WriteLine("FOUT: inkoopfactuur op een debiteur werd niet geweigerd!");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"OK, geweigerd (regel 7): {ex.Message}");
        }

        try
        {
            var leeg = Invoice.Create(relation.Id, relation.Type, InvoiceType.Sales,
                "TEST", invoiceDate, invoiceDate.AddDays(30));
            leeg.Send(); // geen regels -> moet falen
            Console.WriteLine("FOUT: lege factuur werd toch verzonden!");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"OK, geweigerd (geen regels): {ex.Message}");
        }
    }
}
