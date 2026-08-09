using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class PhasesApiClient : IPhasesApiClient
{
    private readonly HttpClient _http;

    public PhasesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Phase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Phase>>("api/phases/getall", cancellationToken);
            return items ?? Array.Empty<Phase>();
        }
        catch
        {
            return Array.Empty<Phase>();
        }
    }

    public async Task<IEnumerable<Phase>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Phase>>("api/phases/active", cancellationToken);
            return items ?? Array.Empty<Phase>();
        }
        catch
        {
            return Array.Empty<Phase>();
        }
    }

    public async Task<Phase?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Phase>($"api/phases/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }
    public async Task<Phase?> CreateAsync(Phase dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/phases", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Phase>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, Phase dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/phases/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
