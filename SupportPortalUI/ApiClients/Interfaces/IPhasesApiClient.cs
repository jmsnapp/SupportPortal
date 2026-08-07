using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IPhasesApiClient
{
    Task<IEnumerable<ReferenceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ReferenceDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<ReferenceDto?> CreateAsync(ReferenceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, ReferenceDto dto, CancellationToken cancellationToken = default);
}
