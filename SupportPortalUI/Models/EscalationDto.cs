namespace SupportPortalUI.Models;

public sealed record EscalationDto
{
    public Int64 Id { get; set; }
    public string? Name { get; set; }
    public string? ProblemSummary { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CustomerImpact { get; set; }
}
