using Microsoft.Extensions.DependencyInjection;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Default for every entity type.
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Entities whose mappers walk navigations. A closed registration takes precedence
        // over the open-generic fallback, so IGenericRepository<TicketEntity> resolves to
        // TicketRepository and picks up its WithDetail() override.
        services.AddScoped<IGenericRepository<CustomerEntity>, CustomerRepository>();
        services.AddScoped<IGenericRepository<IntegrationEntity>, IntegrationRepository>();
        services.AddScoped<IGenericRepository<IntegrationErrorEntity>, IntegrationErrorRepository>();
        services.AddScoped<IGenericRepository<ProjectEntity>, ProjectRepository>();
        services.AddScoped<IGenericRepository<LinkProjectPhaseEntity>, LinkProjectPhaseRepository>();
        services.AddScoped<IGenericRepository<TicketEntity>, TicketRepository>();

        return services;

    }

}
