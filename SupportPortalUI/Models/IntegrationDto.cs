namespace SupportPortalUI.Models;

public sealed record IntegrationDto
{
    public Int64 Id { get; init; }
    public string? Name { get; init; }
    public string? IntegrationType { get; init; }
    public string? CurrentStatus { get; init; }
    public DateTime? LastSuccessfulSync { get; init; }
    public int RetryCount { get; init; }
    public bool IsHealthy => string.Equals(CurrentStatus, "Healthy", StringComparison.OrdinalIgnoreCase);
}
