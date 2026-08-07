using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class IntegrationTypesApiClient : IIntegrationTypesApiClient
{
    private readonly HttpClient _http;

    public IntegrationTypesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ReferenceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ReferenceDto>>("api/integrationtypes/getall", cancellationToken);
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
            var resp = await _http.PostAsJsonAsync("api/integrationtypes", dto, cancellationToken);
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
            var resp = await _http.PutAsJsonAsync($"api/integrationtypes/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
