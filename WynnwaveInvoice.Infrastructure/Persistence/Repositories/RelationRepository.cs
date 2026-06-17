using Microsoft.EntityFrameworkCore;
using WynnwaveInvoice.Application.Relations;
using WynnwaveInvoice.Domain.Relations;

namespace WynnwaveInvoice.Infrastructure.Persistence.Repositories;

public sealed class RelationRepository : IRelationRepository
{
    private readonly ApplicationDbContext _db;

    public RelationRepository(ApplicationDbContext db) => _db = db;

    public async Task<Relation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Relations
            .Include(r => r.ContactPersons)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<RelationOption>> GetSelectableAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<Relation> query = _db.Relations;   // als IQueryable i.v.m. de async-overloadbotsing
        return await query
            .Where(r => r.Type == RelationType.Debtor || r.Type == RelationType.Both)
            .OrderBy(r => r.Name)
            .Select(r => new RelationOption(r.Id, r.Name, r.Address.City))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RelationListItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<Relation> query = _db.Relations;
        return await query
            .OrderBy(r => r.Name)
            .Select(r => new RelationListItem(
                r.Id, r.Name, r.Type, r.Address.City, r.Email, r.ContactPersons.Count))
            .ToListAsync(cancellationToken);
    }

    public void Add(Relation relation) => _db.Relations.Add(relation);
}
