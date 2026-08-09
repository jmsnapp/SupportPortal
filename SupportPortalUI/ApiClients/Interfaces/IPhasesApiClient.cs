using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IPhasesApiClient
{
    Task<IEnumerable<Phase>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Phase>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<Phase?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<Phase?> CreateAsync(Phase dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, Phase dto, CancellationToken cancellationToken = default);
}
