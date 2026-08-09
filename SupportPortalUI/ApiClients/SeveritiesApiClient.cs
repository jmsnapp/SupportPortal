using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class SeveritiesApiClient : ISeveritiesApiClient
{
    private readonly HttpClient _http;

    public SeveritiesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Severity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Severity>>("api/severities/getall", cancellationToken);
            return items ?? Array.Empty<Severity>();
        }
        catch
        {
            return Array.Empty<Severity>();
        }
    }
    public async Task<Severity?> CreateAsync(Severity dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/severities", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Severity>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, Severity dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/severities/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
