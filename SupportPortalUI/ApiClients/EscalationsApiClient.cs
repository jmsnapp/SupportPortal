using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class EscalationsApiClient : IEscalationsApiClient
{
    private readonly HttpClient _http;

    public EscalationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Escalation>> GetActiveAsync(int take = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Escalation>>("api/escalations/active", cancellationToken);
            return items ?? Array.Empty<Escalation>();
        }
        catch
        {
            return Array.Empty<Escalation>();
        }
    }

    public async Task<Escalation?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Escalation>($"api/escalations/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Escalation?> CreateAsync(Escalation dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/escalations", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Escalation>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
