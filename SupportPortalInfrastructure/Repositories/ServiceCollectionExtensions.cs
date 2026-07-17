using Microsoft.Extensions.DependencyInjection;

namespace SupportPortalInfrastructure.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Generic repository for basic CRUD on all entities
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register specialized repositories here
        services.AddScoped<IProjectNoteRepository, ProjectNoteRepository>();
        services.AddScoped<ITicketNoteRepository, TicketNoteRepository>();

        return services;
    }
}