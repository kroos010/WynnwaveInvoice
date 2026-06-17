namespace WynnwaveInvoice.Application.Common;

/// <summary>
/// Bevestigt alle wijzigingen van één use-case in één transactie. De
/// domain-event-dispatch (interceptor) vuurt op deze SaveChangesAsync.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
