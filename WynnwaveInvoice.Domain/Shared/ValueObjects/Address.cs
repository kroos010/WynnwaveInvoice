using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Shared.ValueObjects;

public sealed record Address
{
    public string StreetName { get; }
    public string HouseNumber { get; }
    public string? HouseNumberAddition { get; }
    public string PostalCode { get; }
    public string City { get; }
    public string Country { get; }

    public Address(string streetName, string houseNumber, string? houseNumberAddition, string postalCode, string city, string country)
    {
        if (string.IsNullOrWhiteSpace(streetName)) throw new DomainException("Straatnaam is verplicht.");
        if (string.IsNullOrWhiteSpace(houseNumber)) throw new DomainException("Huisnummer is verplicht.");
        if (string.IsNullOrWhiteSpace(postalCode)) throw new DomainException("Postcode is verplicht.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("Plaats is verplicht.");
        if (string.IsNullOrWhiteSpace(country)) throw new DomainException("Land is verplicht.");

        StreetName = streetName;
        HouseNumber = houseNumber;
        HouseNumberAddition = houseNumberAddition;
        PostalCode = postalCode;
        City = city;
        Country = country;
    }

    private Address() { StreetName = null!; HouseNumber = null!; PostalCode = null!; City = null!; Country = null!; } // EF
}
