namespace SupportPortalUI.Models;

public sealed record TicketDto
{
    public Int64 Id { get; set; }
    public string? Name { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedDate { get; set; }
    public Int64? EscalationId { get; set; }
}
