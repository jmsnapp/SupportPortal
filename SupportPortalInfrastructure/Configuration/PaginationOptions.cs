namespace SupportPortalInfrastructure.Configuration;

public class PaginationOptions
{
    /// <summary>
    /// Maximum page size enforced by repositories. Defaults to 200 when not configured.
    /// </summary>
    public int MaxPageSize { get; set; } = 200;

    /// <summary>
    /// Default page size that controllers may use when not provided by callers.
    /// This value is advisory; repositories enforce MaxPageSize.
    /// </summary>
    public int DefaultPageSize { get; set; } = 50;
}
