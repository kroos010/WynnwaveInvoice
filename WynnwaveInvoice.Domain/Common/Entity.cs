namespace WynnwaveInvoice.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public override bool Equals(object? obj) =>
        obj is Entity other && GetType() == other.GetType() && Id == other.Id && Id != default;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
