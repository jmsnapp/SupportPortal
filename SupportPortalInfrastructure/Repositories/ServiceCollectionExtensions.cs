using Microsoft.Extensions.DependencyInjection;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGenericRepository<Industry, Industry, IndustryEntity>, IndustryRepository>();
        services.AddScoped<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>, IntegrationStatusRepository>();
        services.AddScoped<IGenericRepository<IntegrationType, IntegrationType, IntegrationTypeEntity>, IntegrationTypeRepository>();
        services.AddScoped<IGenericRepository<Phase, Phase, PhaseEntity>, PhaseRepository>();
        services.AddScoped<IGenericRepository<Severity, Severity, SeverityEntity>, SeverityRepository>();
        services.AddScoped<IGenericRepository<SupportStatus, SupportStatus, SupportStatusEntity>, SupportStatusRepository>();
        services.AddScoped<IGenericRepository<Escalation, EscalationListItem, EscalationEntity>, EscalationRepository>();
        services.AddScoped<IGenericRepository<Customer, CustomerListItem, CustomerEntity>, CustomerRepository>();
        services.AddScoped<IGenericRepository<Integration, IntegrationListItem, IntegrationEntity>, IntegrationRepository>();
        services.AddScoped<IGenericRepository<IntegrationError, IntegrationErrorListItem, IntegrationErrorEntity>, IntegrationErrorRepository>();
        services.AddScoped<IGenericRepository<Project, ProjectList, ProjectEntity>, ProjectRepository>();
        services.AddScoped<IGenericRepository<ProjectPhase, ProjectPhaseListItem, LinkProjectPhaseEntity>, ProjectPhaseRepository>();
        services.AddScoped<IGenericRepository<ProjectNote, ProjectNote, ProjectNoteEntity>, ProjectNoteRepository>();
        services.AddScoped<IGenericRepository<Ticket, TicketListItem, TicketEntity>, TicketRepository>();
        services.AddScoped<IGenericRepository<TicketNote, TicketNote, TicketNoteEntity>, TicketNoteRepository>();

        return services;

    }

}
