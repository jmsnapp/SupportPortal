using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IntegrationTypesApiClient : IIntegrationTypesApiClient
{
    private readonly HttpClient _http;

    public IntegrationTypesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<IntegrationType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<IntegrationType>>("api/integrationtypes/getall", cancellationToken);
            return items ?? Array.Empty<IntegrationType>();
        }
        catch
        {
            return Array.Empty<IntegrationType>();
        }
    }
    public async Task<IntegrationType?> CreateAsync(IntegrationType dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/integrationtypes", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<IntegrationType>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, IntegrationType dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/integrationtypes/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
