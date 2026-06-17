using Microsoft.Extensions.DependencyInjection;
using WynnwaveInvoice.Application.Invoices;
using WynnwaveInvoice.Application.Relations;

namespace WynnwaveInvoice.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RelationService>();
        services.AddScoped<InvoiceService>();
        return services;
    }
}
