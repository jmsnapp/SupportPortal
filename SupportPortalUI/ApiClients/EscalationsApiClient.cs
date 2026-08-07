using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class EscalationsApiClient : IEscalationsApiClient
{
    private readonly HttpClient _http;

    public EscalationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<EscalationDto>> GetActiveAsync(int take = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<EscalationDto>>("api/escalations/active", cancellationToken);
            return items ?? Array.Empty<EscalationDto>();
        }
        catch
        {
            return Array.Empty<EscalationDto>();
        }
    }

    public async Task<EscalationDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<EscalationDto>($"api/escalations/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<EscalationDto?> CreateAsync(EscalationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/escalations", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<EscalationDto>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
