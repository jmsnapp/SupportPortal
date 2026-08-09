using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IProjectNotesApiClient
{
    Task<IEnumerable<ProjectNote>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);
    Task<ProjectNote?> CreateAsync(ProjectNote note, CancellationToken cancellationToken = default);
}
