using WynnwaveInvoice.Domain.Relations;

namespace WynnwaveInvoice.Application.Relations;

public interface IRelationRepository
{
    /// <summary>Volledige aggregate incl. contactpersonen.</summary>
    Task<Relation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lichte projectie voor keuzelijsten (debiteuren + beide).</summary>
    Task<IReadOnlyList<RelationOption>> GetSelectableAsync(CancellationToken cancellationToken = default);

    /// <summary>Lijstweergave met type, plaats en aantal contactpersonen.</summary>
    Task<IReadOnlyList<RelationListItem>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(Relation relation);
}

public sealed record RelationOption(Guid Id, string Name, string City);

public sealed record RelationListItem(
    Guid Id, string Name, RelationType Type, string City, string? Email, int ContactCount);

