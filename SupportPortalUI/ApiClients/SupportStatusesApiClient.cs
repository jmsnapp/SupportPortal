using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class SupportStatusesApiClient : ISupportStatusesApiClient
{
    private readonly HttpClient _http;

    public SupportStatusesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ReferenceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ReferenceDto>>("api/supportstatuses/getall", cancellationToken);
            return items ?? Array.Empty<ReferenceDto>();
        }
        catch
        {
            return Array.Empty<ReferenceDto>();
        }
    }
    public async Task<ReferenceDto?> CreateAsync(ReferenceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/supportstatuses", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ReferenceDto>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, ReferenceDto dto, CancellationToken cancellationToken = default)
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
