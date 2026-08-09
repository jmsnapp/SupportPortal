using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IntegrationStatusesApiClient : IIntegrationStatusesApiClient
{
    private readonly HttpClient _http;

    public IntegrationStatusesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<IntegrationStatus>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<IntegrationStatus>>("api/integrationstatuses/getall", cancellationToken);
            return items ?? Array.Empty<IntegrationStatus>();
        }
        catch
        {
            return Array.Empty<IntegrationStatus>();
        }
    }

    public async Task<IntegrationStatus?> CreateAsync(IntegrationStatus dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/integrationstatuses", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<IntegrationStatus>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, IntegrationStatus dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/integrationstatuses/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
