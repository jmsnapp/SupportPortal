using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IntegrationsApiClient : IIntegrationsApiClient
{
    private readonly HttpClient _http;

    public IntegrationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<IntegrationDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<IntegrationDto>>("api/integrations/active", cancellationToken);
            return items ?? Array.Empty<IntegrationDto>();
        }
        catch
        {
            return Array.Empty<IntegrationDto>();
        }
    }

    public async Task<IntegrationDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<IntegrationDto>($"api/integrations/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
