using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WynnwaveInvoice.Domain.Invoices;
using WynnwaveInvoice.Infrastructure.Invoices;
using WynnwaveInvoice.Infrastructure.Persistence;

namespace WynnwaveInvoice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
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
