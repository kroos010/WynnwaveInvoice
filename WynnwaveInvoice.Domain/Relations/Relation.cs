using WynnwaveInvoice.Domain.Common;
using WynnwaveInvoice.Domain.Relations.Events;
using WynnwaveInvoice.Domain.Shared.ValueObjects;

namespace WynnwaveInvoice.Domain.Relations;

public sealed class Relation : AggregateRoot, IAuditable
{
    private readonly List<ContactPerson> _contactPersons = new();
    public IReadOnlyCollection<ContactPerson> ContactPersons => _contactPersons.AsReadOnly();

    public RelationType Type { get; private set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public string? VATNumber { get; private set; }
    public string? ChamberOfCommerceNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? IBAN { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Relation(RelationType type, string name, Address address) : this()
    {
        Type = type;
        Name = name;
        Address = address;
    }
    
    private Relation() { Name = null!; Address = null!; } // EF

    public static Relation Create(RelationType type, string name, Address address, string? email = null, string? phone = null, string? vatNumber = null, string? chamberOfCommerceNumber = null, string? iban = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Naam van de relatie is verplicht.");

        var relation = new Relation(type, name, address)
        {
            Email = email,
            Phone = phone,
            VATNumber = vatNumber,
            ChamberOfCommerceNumber = chamberOfCommerceNumber,
            IBAN = iban,
            Notes = notes
        };

        relation.Raise(new RelationCreated(relation.Id, type, name));

        return relation;
    }

    public ContactPerson AddContactPerson(string firstName, string lastName, string? email = null, string? phone = null, string? jobTitle = null, bool isPrimaryContact = false)
    {
        if (isPrimaryContact)
            foreach (var existing in _contactPersons)
                existing.UnsetPrimary();

        var contactPerson = ContactPerson.Create(Id, firstName, lastName, email, phone, jobTitle, isPrimaryContact);
        _contactPersons.Add(contactPerson);

        Raise(new ContactPersonAdded(Id, contactPerson.Id));
        return contactPerson;
    }
    
    public void RemoveContactPerson(Guid contactPersonId)
    {
        var contact = _contactPersons.FirstOrDefault(c => c.Id == contactPersonId);
        if (contact is null) return; // idempotent
        _contactPersons.Remove(contact);
    }

    public void UpdateContactPerson(Guid contactPersonId, string firstName, string lastName, string? email, string? phone, string? jobTitle, bool isPrimaryContact)
    {
        var contact = _contactPersons.FirstOrDefault(c => c.Id == contactPersonId)
                      ?? throw new DomainException("Contactpersoon niet gevonden.");

        if (isPrimaryContact)
            foreach (var other in _contactPersons.Where(c => c.Id != contactPersonId))
                other.UnsetPrimary();

        contact.Update(firstName, lastName, email, phone, jobTitle, isPrimaryContact);
    }

    public void UpdateDetails(string name, Address address, string? email, string? phone, string? vatNumber, string? chamberOfCommerceNumber, string? iban, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Naam van de relatie is verplicht.");

        Name = name;
        Address = address;
        Email = email;
        Phone = phone;
        VATNumber = vatNumber;
        ChamberOfCommerceNumber = chamberOfCommerceNumber;
        IBAN = iban;
        Notes = notes;
    }

}
