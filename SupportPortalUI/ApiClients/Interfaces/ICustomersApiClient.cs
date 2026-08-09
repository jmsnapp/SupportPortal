using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ICustomersApiClient
{
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<Customer?> CreateAsync(Customer dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, Customer dto, CancellationToken cancellationToken = default);
}
