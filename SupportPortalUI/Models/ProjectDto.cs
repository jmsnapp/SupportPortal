namespace SupportPortalUI.Models;

public sealed record ProjectDto
{
    public Int64 Id { get; init; }
    public string? Name { get; init; }
    public string? CurrentPhase { get; init; }
    public DateTime? TargetGoLiveDate { get; init; }
}
