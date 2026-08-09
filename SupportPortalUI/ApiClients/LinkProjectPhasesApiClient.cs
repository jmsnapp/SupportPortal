using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class LinkProjectPhasesApiClient : ILinkProjectPhasesApiClient
{
    private readonly HttpClient _http;

    public LinkProjectPhasesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProjectPhase>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ProjectPhase>>($"api/linkprojectphases?projectId={projectId}", cancellationToken);
            return items ?? Array.Empty<ProjectPhase>();
        }
        catch
        {
            return Array.Empty<ProjectPhase>();
        }
    }

    public async Task<ProjectPhase?> CreateAsync(ProjectPhase link, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/linkprojectphases", link, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ProjectPhase>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.DeleteAsync($"api/linkprojectphases/{id}", cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
