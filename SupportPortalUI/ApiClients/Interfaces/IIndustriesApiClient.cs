using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IIndustriesApiClient
{
    Task<IEnumerable<Industry>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Industry?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<Industry?> CreateAsync(Industry dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, Industry dto, CancellationToken cancellationToken = default);
}
