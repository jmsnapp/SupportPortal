using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IndustriesApiClient : IIndustriesApiClient
{
    private readonly HttpClient _http;

    public IndustriesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Industry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Industry>>("api/industries/getall", cancellationToken);
            return items ?? Array.Empty<Industry>();
        }
        catch
        {
            return Array.Empty<Industry>();
        }
    }

    public async Task<Industry?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Industry>($"api/industries/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Industry?> CreateAsync(Industry dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/industries", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Industry>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, Industry dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/industries/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
