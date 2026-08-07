namespace SupportPortalUI.Models;

public sealed record LinkProjectPhaseDto
{
    public Int64 Id { get; init; }
    public Int64 ProjectId { get; init; }
    public Int64 PhaseId { get; init; }
    public bool Deleted { get; init; }
}
