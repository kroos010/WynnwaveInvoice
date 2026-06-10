using Microsoft.Extensions.DependencyInjection;
using WynnwaveInvoice.Domain.Common;

namespace WynnwaveInvoice.Infrastructure.Persistence;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

        foreach (var handler in _serviceProvider.GetServices(handlerType))
        {
            if (handler is null) continue;
            await (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
        }
    }
}
