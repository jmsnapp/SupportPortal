using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IntegrationsApiClient : IIntegrationsApiClient
{
    private readonly HttpClient _http;

    public IntegrationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Integration>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Integration>>("api/integrations/active", cancellationToken);
            return items ?? Array.Empty<Integration>();
        }
        catch
        {
            return Array.Empty<Integration>();
        }
    }

    public async Task<Integration?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Integration>($"api/integrations/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
