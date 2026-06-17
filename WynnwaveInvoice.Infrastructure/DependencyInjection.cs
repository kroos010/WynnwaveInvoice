using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WynnwaveInvoice.Application.Common;
using WynnwaveInvoice.Application.Invoices;
using WynnwaveInvoice.Application.Relations;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Infrastructure.Invoices;
using WynnwaveInvoice.Infrastructure.Persistence;
using WynnwaveInvoice.Infrastructure.Persistence.Repositories;

namespace WynnwaveInvoice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRelationRepository, RelationRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        
        //services.AddSingleton<AuditableEntityInterceptor>();
        //services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        //services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention()       // pakket: EFCore.NamingConventions
                   .AddInterceptors(
                       //sp.GetRequiredService<AuditableEntityInterceptor>(),
                       //sp.GetRequiredService<DomainEventDispatchInterceptor>()
                       );
        });

        services.AddScoped<IInvoiceNumberGenerator, InvoiceNumberGenerator>();

        return services;
    }
}
