namespace SupportPortalUI.Models;

public sealed record TicketNoteDto
{
    public Int64 Id { get; init; }
    public Int64 TicketId { get; init; }
    public string? Author { get; init; }
    public string? Text { get; init; }
    public DateTime CreatedDate { get; init; }
}
