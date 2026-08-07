namespace SupportPortalUI.Models;

public sealed record ProjectNoteDto
{
    public Int64 Id { get; init; }
    public Int64 ProjectId { get; init; }
    public string? Author { get; init; }
    public string? Text { get; init; }
    public DateTime CreatedDate { get; init; }
}
