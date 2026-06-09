using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Relations.Events;

public sealed record RelationCreated(Guid RelationId, RelationType Type, string Name) : DomainEvent;
