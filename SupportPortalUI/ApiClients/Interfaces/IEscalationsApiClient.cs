using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IEscalationsApiClient
{
    Task<IEnumerable<EscalationDto>> GetActiveAsync(int take = 5, CancellationToken cancellationToken = default);
    Task<EscalationDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<EscalationDto?> CreateAsync(EscalationDto dto, CancellationToken cancellationToken = default);
}
