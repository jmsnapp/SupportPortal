using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalDomain.Models;

public class PortalObject
{
    public Int64 Id { get; set; }

    [StringLength(63), Required]
    public string Name { get; set; }

    [StringLength(255), Required]
    public  virtual string Description { get; set; }

    [Required]
    public bool Deleted { get; set; }

    /// <summary>
    /// Concurrency token. Send back whatever a read handed you, unchanged; an update that
    /// carries a stale token is rejected with 409 instead of silently overwriting.
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public PortalObject()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class Customer : PortalObject
{
    public Industry Industry { get; set; }

    [StringLength(63), Required]
    public string PrimaryContact { get; set; }

    [StringLength(63), Required, EmailAddress]
    public string PrimaryContactEmail { get; set; }

    [StringLength(63), Required]
    public string TechnicalContact { get; set; }

    [StringLength(63), Required, EmailAddress]
    public string TechnicalContactEmail { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public Customer()
    {
        Industry = new Industry();
        PrimaryContact = string.Empty;
        PrimaryContactEmail = string.Empty;
        TechnicalContact = string.Empty;
        TechnicalContactEmail = string.Empty;
        CreatedDate = DateTime.UtcNow;

    }

}

public class Escalation  : PortalObject
{
    [Required]
    public string ProblemSummary { get; set; }

    [Required]
    public string CustomerImpact { get; set; }

    public string RootCause { get; set; }

    public string RecommendedActions { get; set; }

    public DateTime CreatedDate { get; set; }

    public Escalation()
    {
        ProblemSummary = string.Empty;
        CustomerImpact = string.Empty;
        RootCause = string.Empty;
        RecommendedActions = string.Empty;
        CreatedDate = DateTime.UtcNow;

    }

}

public class Industry : PortalObject
{ }

public class Integration : PortalObject
{
    [StringLength(127)]
    public override string Description { get; set; }

    public Customer Customer { get; set; }

    public IntegrationType Type { get; set; }

    public IntegrationStatus CurrentStatus { get; set; }

    public DateTime LastSuccessfulSync { get; set; }

    public DateTime LastFailedSync { get; set; }

    public int RetryCount { get; set; }

    public Integration()
    {
        Description = string.Empty;
        Customer = new Customer();
        Type = new IntegrationType();
        CurrentStatus = new IntegrationStatus();
        LastSuccessfulSync = new DateTime(1900, 1, 1);
        LastFailedSync = new DateTime(1900, 1, 1);
        RetryCount = 0;

    }

}

public class IntegrationError : PortalObject
{
    public Integration Integration { get; set; }

    [StringLength(1027)]
    public string ErrorMessage { get; set; }

    public string StackTrace { get; set; }

    [Required]
    public DateTime ErrorTime { get; set; }

    public IntegrationError()
    {
        Integration = new Integration();
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
        ErrorTime = DateTime.UtcNow;

    }

}

public class IntegrationStatus : PortalObject
{ }

public class IntegrationType : PortalObject
{ }

public class ProjectPhase : PortalObject
{
    [Required]
    public Int64 ProjectId { get; set; }

    public Phase Phase { get; set; }

    public decimal Percentage { get; set; }

    public int Order { get; set; }

    public ProjectPhase()
    {
        ProjectId = 0;
        Phase = new Phase();
        Percentage = 0;
        Order = 0;

    }

}

public class Phase : PortalObject
{ }

public class Project : PortalObject
{
    public Customer Customer { get; set; }

    public Phase CurrentPhase { get; set; }

    [Required]
    public DateTime TargetGoLive { get; set; }

    public DateTime ActualGoLive { get; set; }

    public List<ProjectPhase> Phases { get; set; }

    public List<ProjectNote> Notes { get; set; }

    public Project()
    {
        Customer = new Customer();
        CurrentPhase = new Phase();
        TargetGoLive = DateTime.UtcNow;
        ActualGoLive = new DateTime(1900, 1, 1);
        Phases = new List<ProjectPhase>();
        Notes = new List<ProjectNote>();

    }

}

public class ProjectNote : PortalObject
{
    [Required]
    public Int64 ProjectId { get; set; }

    [Required]
    public string Note { get; set; }

    public DateTime CreateTime { get; set; }

    public ProjectNote()
    {
        ProjectId = 0;
        Note = string.Empty;
        CreateTime = DateTime.UtcNow;

    }

}

public class Severity : PortalObject
{ }

public class SupportStatus : PortalObject
{ }

public class Ticket : PortalObject
{

    [StringLength(1023)]
    public override string Description { get; set; }

    public Customer Customer { get; set; }

    public Integration Integration { get; set; }

    public Severity Severity { get; set; }

    public Escalation Escalation { get; set; }

    public SupportStatus Status { get; set; }

    public string Reproduce { get; set; }

    [Required, StringLength(63)]
    public string ReportedBy { get; set; }

    [Required, StringLength(63)]
    public string AssignedTo { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime ResolutionDate { get; set; }

    public string Resolution { get; set; }

    public List<TicketNote> Notes { get; set; }

    public Ticket()
    {
        Description = string.Empty;
        Customer = new Customer();
        Integration = new Integration();
        Severity = new Severity();
        Escalation = new Escalation();
        Status = new SupportStatus();
        Reproduce = string.Empty;
        ReportedBy = string.Empty;
        AssignedTo = string.Empty;
        Resolution = string.Empty;
        CreatedDate = DateTime.UtcNow;
        ResolutionDate = new DateTime(1900, 1, 1);
        Resolution = string.Empty;
        Notes = new List<TicketNote>();

    }

}

public class TicketNote : PortalObject
{
    [Required]
    public Int64 TicketId { get; set; }

    [Required]
    public string Note { get; set; }

    public DateTime CreateTime { get; set; }

    public TicketNote()
    {
        TicketId = 0;
        Note = string.Empty;
        CreateTime = DateTime.UtcNow;

    }

}
