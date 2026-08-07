using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class ProjectsApiClient : IProjectsApiClient
{
    private readonly HttpClient _http;

    public ProjectsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProjectDto>> GetActiveAsync(int take = 8, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ProjectDto>>("api/projects/active", cancellationToken);
            return items?.Take(take) ?? Array.Empty<ProjectDto>();
        }
        catch
        {
            return Array.Empty<ProjectDto>();
        }
    }

    public async Task<ProjectDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProjectDto>($"api/projects/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
