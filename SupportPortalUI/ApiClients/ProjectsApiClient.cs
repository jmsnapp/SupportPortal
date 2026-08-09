using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class ProjectsApiClient : IProjectsApiClient
{
    private readonly HttpClient _http;

    public ProjectsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Project>> GetActiveAsync(int take = 8, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Project>>("api/projects/active", cancellationToken);
            return items?.Take(take) ?? Array.Empty<Project>();
        }
        catch
        {
            return Array.Empty<Project>();
        }
    }

    public async Task<Project?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Project>($"api/projects/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
