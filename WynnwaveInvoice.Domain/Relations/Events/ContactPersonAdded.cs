using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Domain.Relations.Events;

public sealed record ContactPersonAdded(Guid RelationId, Guid ContactPersonId) : DomainEvent;
