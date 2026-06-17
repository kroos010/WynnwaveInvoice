using WynnwaveInvoice.Application.Common;
using WynnwaveInvoice.Domain.Relations;
using WynnwaveInvoice.Domain.Shared.ValueObjects;

namespace WynnwaveInvoice.Application.Relations;

public sealed class RelationService
{
    private readonly IRelationRepository _relations;
    private readonly IUnitOfWork _unitOfWork;

    public RelationService(IRelationRepository relations, IUnitOfWork unitOfWork)
    {
        _relations = relations;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<RelationOption>> GetSelectableAsync(CancellationToken ct = default)
        => _relations.GetSelectableAsync(ct);

    public Task<IReadOnlyList<RelationListItem>> GetAllAsync(CancellationToken ct = default)
        => _relations.GetAllAsync(ct);

    public async Task<RelationDetail?> GetForEditAsync(Guid id, CancellationToken ct = default)
    {
        var relation = await _relations.GetByIdAsync(id, ct);
        return relation is null ? null : Map(relation);
    }

    public async Task<Guid> CreateAsync(CreateRelationRequest request, CancellationToken ct = default)
    {
        var relation = Relation.Create(
            request.Type, request.Name,
            new Address(request.StreetName, request.HouseNumber, request.HouseNumberAddition,
                        request.PostalCode, request.City, request.Country),
            request.Email, request.Phone, request.VatNumber, request.ChamberOfCommerceNumber,
            request.Iban, request.Notes);

        foreach (var c in request.Contacts)
            relation.AddContactPerson(c.FirstName, c.LastName, c.Email, c.Phone, c.JobTitle, c.IsPrimary);

        _relations.Add(relation);
        await _unitOfWork.SaveChangesAsync(ct);
        return relation.Id;
    }

    public async Task UpdateAsync(UpdateRelationRequest request, CancellationToken ct = default)
    {
        var relation = await _relations.GetByIdAsync(request.Id, ct)
            ?? throw new InvalidOperationException("Relatie niet gevonden.");

        relation.UpdateDetails(
            request.Name,
            new Address(request.StreetName, request.HouseNumber, request.HouseNumberAddition,
                        request.PostalCode, request.City, request.Country),
            request.Email, request.Phone, request.VatNumber, request.ChamberOfCommerceNumber,
            request.Iban, request.Notes);

        // contactpersonen verzoenen: verwijderen wat weg is, bijwerken wat blijft, toevoegen wat nieuw is
        var keepIds = request.Contacts.Where(c => c.Id is not null).Select(c => c.Id!.Value).ToHashSet();
        foreach (var existing in relation.ContactPersons.Where(c => !keepIds.Contains(c.Id)).ToList())
            relation.RemoveContactPerson(existing.Id);

        foreach (var c in request.Contacts)
        {
            if (c.Id is null)
                relation.AddContactPerson(c.FirstName, c.LastName, c.Email, c.Phone, c.JobTitle, c.IsPrimary);
            else
                relation.UpdateContactPerson(c.Id.Value, c.FirstName, c.LastName, c.Email, c.Phone, c.JobTitle, c.IsPrimary);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static RelationDetail Map(Relation r) => new(
        r.Id, r.Type, r.Name,
        r.Address.StreetName, r.Address.HouseNumber, r.Address.HouseNumberAddition,
        r.Address.PostalCode, r.Address.City, r.Address.Country,
        r.Email, r.Phone, r.VATNumber, r.ChamberOfCommerceNumber, r.IBAN, r.Notes,
        r.ContactPersons
            .OrderByDescending(c => c.IsPrimaryContact).ThenBy(c => c.LastName)
            .Select(c => new RelationContactDetail(
                c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.JobTitle, c.IsPrimaryContact))
            .ToList());
}

// ---- read model voor de bewerkpagina ----
public sealed record RelationDetail(
    Guid Id, RelationType Type, string Name,
    string StreetName, string HouseNumber, string? HouseNumberAddition,
    string PostalCode, string City, string Country,
    string? Email, string? Phone, string? VatNumber, string? ChamberOfCommerceNumber,
    string? Iban, string? Notes,
    IReadOnlyList<RelationContactDetail> Contacts);

public sealed record RelationContactDetail(
    Guid Id, string FirstName, string LastName, string? Email, string? Phone, string? JobTitle, bool IsPrimary);

// ---- requests ----
public sealed record RelationContactInput(
    Guid? Id, string FirstName, string LastName,
    string? Email, string? Phone, string? JobTitle, bool IsPrimary);

public sealed record CreateRelationRequest(
    RelationType Type, string Name,
    string StreetName, string HouseNumber, string? HouseNumberAddition,
    string PostalCode, string City, string Country,
    string? Email, string? Phone, string? VatNumber, string? ChamberOfCommerceNumber,
    string? Iban, string? Notes,
    IReadOnlyList<RelationContactInput> Contacts);

public sealed record UpdateRelationRequest(
    Guid Id, string Name,
    string StreetName, string HouseNumber, string? HouseNumberAddition,
    string PostalCode, string City, string Country,
    string? Email, string? Phone, string? VatNumber, string? ChamberOfCommerceNumber,
    string? Iban, string? Notes,
    IReadOnlyList<RelationContactInput> Contacts);
