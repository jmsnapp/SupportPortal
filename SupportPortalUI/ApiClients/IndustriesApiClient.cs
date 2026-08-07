using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IndustriesApiClient : IIndustriesApiClient
{
    private readonly HttpClient _http;

    public IndustriesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ReferenceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ReferenceDto>>("api/industries/getall", cancellationToken);
            return items ?? Array.Empty<ReferenceDto>();
        }
        catch
        {
            return Array.Empty<ReferenceDto>();
        }
    }

    public async Task<ReferenceDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ReferenceDto>($"api/industries/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReferenceDto?> CreateAsync(ReferenceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/industries", dto, cancellationToken);
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
            var resp = await _http.PutAsJsonAsync($"api/industries/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
