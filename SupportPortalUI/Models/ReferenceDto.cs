namespace SupportPortalUI.Models;

public sealed record ReferenceDto
{
    public Int64 Id { get; set; }
    public string? Name { get; set; }
    public bool Deleted { get; set; }
}
