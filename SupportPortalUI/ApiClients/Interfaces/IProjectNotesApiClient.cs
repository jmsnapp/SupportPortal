using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IProjectNotesApiClient
{
    Task<IEnumerable<ProjectNoteDto>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);
    Task<ProjectNoteDto?> CreateAsync(ProjectNoteDto note, CancellationToken cancellationToken = default);
}
