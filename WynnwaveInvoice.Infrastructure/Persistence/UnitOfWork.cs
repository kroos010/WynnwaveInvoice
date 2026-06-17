using WynnwaveInvoice.Application.Common;

namespace WynnwaveInvoice.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
 
    public UnitOfWork(ApplicationDbContext db) => _db = db;
 
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}

