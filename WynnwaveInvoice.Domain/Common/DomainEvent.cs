namespace WynnwaveInvoice.Domain.Common;

/// <summary>Gemak-basis zodat events alleen hun payload hoeven te declareren.</summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}