using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Relations;

public sealed class ContactPerson : Entity, IAuditable
{
    public Guid RelationId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? JobTitle { get; private set; }
    public bool IsPrimaryContact { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ContactPerson() { FirstName = null!; LastName = null!; } // EF

    private ContactPerson(Guid relationId, string firstName, string lastName, string? email, string? phone, string? jobTitle, bool isPrimaryContact)
    {
        RelationId = relationId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        JobTitle = jobTitle;
        IsPrimaryContact = isPrimaryContact;
    }

    internal static ContactPerson Create(Guid relationId, string firstName, string lastName, string? email, string? phone, string? jobTitle, bool isPrimaryContact)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("Voornaam van de contactpersoon is verplicht.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("Achternaam van de contactpersoon is verplicht.");

        return new ContactPerson(relationId, firstName, lastName, email, phone, jobTitle, isPrimaryContact);
    }

    internal void UnsetPrimary() => IsPrimaryContact = false;
}
