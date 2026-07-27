using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalInfrastructure.Models;

public class PortalObject
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

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

    public string PrimaryContact { get; set; }

    public string PrimaryContactEmail { get; set; }

    public string TechnicalContact { get; set; }

    public string TechnicalContactEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    public Customer()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        Industry = new Industry();
        PrimaryContact = string.Empty;
        PrimaryContactEmail = string.Empty;
        TechnicalContact = string.Empty;
        TechnicalContactEmail = string.Empty;
        CreatedDate = DateTime.Now;

    }

}

public class Escalation  : PortalObject
{
    public string ProblemSummary { get; set; }

    public string? CustomerImpact { get; set; }

    public string? RootCause { get; set; }

    public string? RecommendedAction { get; set; }

    public Escalation()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        ProblemSummary = string.Empty;
        CustomerImpact = string.Empty;
        RootCause = string.Empty;
        RecommendedAction = string.Empty;

    }

}

public class Industry : PortalObject
{ }

public class Integration : PortalObject
{
    public Customer Customer { get; set; }

    public IntegrationType Type { get; set; }

    public IntegrationStatus CurrentStatus { get; set; }

    public DateTime? LastSuccessfulSync { get; set; }

    public DateTime? LastFailedSync { get; set; }

    public int RetryCount { get; set; }

    public Integration()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        Customer = new Customer();
        Type = new IntegrationType();
        CurrentStatus = new IntegrationStatus();
        LastSuccessfulSync = null;
        LastFailedSync = null;
        RetryCount = 0;

    }

}

public class IntegrationError : PortalObject
{
    public Integration Integration { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }

    public DateTime ErrorTime { get; set; }

    public IntegrationError()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        Integration = new Integration();
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
        ErrorTime = DateTime.Now;

    }

}

public class IntegrationStatus : PortalObject
{ }

public class IntegrationType : PortalObject
{ }

public class ProjectPhase : PortalObject
{
    public int ProjectId { get; set; }

    public Phase Phase { get; set; }

    public decimal? Percentage { get; set; }

    public int Order { get; set; }

    public ProjectPhase()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

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
    public Phase CurrentPhase { get; set; }

    public DateTime TargetGoLive { get; set; }

    public DateTime? ActualGoLive { get; set; }

    public List<ProjectPhase> Phases { get; set; }

    public List<ProjectNote> Notes { get; set; }

    public Project()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        CurrentPhase = new Phase();
        TargetGoLive = DateTime.Now;
        Phases = new List<ProjectPhase>();
        Notes = new List<ProjectNote>();

    }

}

public class ProjectNote : PortalObject
{ 
    public int ProjectId { get; set; }
    
    public string Note { get; set; }

    public ProjectNote()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        ProjectId = 0;
        Note = string.Empty;

    }

}

public class Severity : PortalObject
{ }

public class SupportStatus : PortalObject
{ }

public class Ticket : PortalObject
{

    public Customer Customer { get; set; }

    public Integration Integration { get; set; }

    public Severity Severity { get; set; }

    public Escalation? Escalation { get; set; }

    public SupportStatus Status { get; set; }

    public string? Reproduce { get; set; }

    public string ReportedBy { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ResolutionDate { get; set; }

    public string? Resolution { get; set; }

    public List<TicketNote> Notes { get; set; }

    public Ticket()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        Customer = new Customer();
        Integration = new Integration();
        Severity = new Severity();
        Escalation = null;
        Status = new SupportStatus();
        Reproduce = string.Empty;
        ReportedBy = string.Empty;
        AssignedTo = string.Empty;
        CreatedDate = DateTime.Now;
        ResolutionDate = null;
        Resolution = string.Empty;
        Notes = new List<TicketNote>();

    }

}

public class TicketNote : PortalObject
{
    public int TicketId { get; set; }

    public string Note { get; set; }

    public TicketNote()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        TicketId = 0;
        Note = string.Empty;

    }

}
