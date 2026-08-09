using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class SupportStatusesApiClient : ISupportStatusesApiClient
{
    private readonly HttpClient _http;

    public SupportStatusesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<SupportStatus>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<SupportStatus>>("api/supportstatuses/getall", cancellationToken);
            return items ?? Array.Empty<SupportStatus>();
        }
        catch
        {
            return Array.Empty<SupportStatus>();
        }
    }
    public async Task<SupportStatus?> CreateAsync(SupportStatus dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/supportstatuses", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<SupportStatus>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, SupportStatus dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/supportstatuses/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
