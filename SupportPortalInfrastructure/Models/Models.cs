using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalInfrastructure.Models;

public class Industry
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public Industry()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class Customer
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string PrimaryContact { get; set; }

    public string PrimaryContactEmail { get; set; }

    public string TechnicalContact { get; set; }

    public string TechnicalContactEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool Deleted { get; set; }

    public string Description { get; set; }

    public Customer()
    {
        Id = 0;
        Name = string.Empty;
        PrimaryContact = string.Empty;
        PrimaryContactEmail = string.Empty;
        TechnicalContact = string.Empty;
        TechnicalContactEmail = string.Empty;
        CreatedDate = DateTime.Now;
        Description = string.Empty;
        Deleted = false;

    }

}

public class Project
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public int CurrentPhase { get; set; }

    public DateTime TargetGoLive { get; set; }

    public DateTime? ActualGoLive { get; set; }

    public bool Deleted { get; set; }

    public Project()
    {
        Id = 0;
        Name = string.Empty;
        CurrentPhase = 0;
        TargetGoLive = DateTime.Now;
        Deleted = false;

    }

}

public class Phase
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public Phase()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class LinkProjectPhase
{
    [Key]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int PhaseId { get; set; }

    public decimal? Percentage { get; set; }

    public int Order { get; set; }

    public bool Deleted { get; set; }

    public LinkProjectPhase()
    {
        Id = 0;
        ProjectId = 0;
        PhaseId = 0;
        Percentage = 0;
        Order = 0;
        Deleted = false;

    }

}

public class Integration
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public int CustomerId { get; set; }

    public int IntegrationTypeId { get; set; }

    public int CurrentStatusId { get; set; }

    public DateTime? LastSuccessfulSync { get; set; }

    public DateTime? LastFailedSync { get; set; }

    public int RetryCount { get; set; }

    public Integration()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;
        CustomerId = 0;
        IntegrationTypeId = 0;
        CurrentStatusId = 0;
        LastSuccessfulSync = null;
        LastFailedSync = null;
        RetryCount = 0;

    }

}

public class IntegrationStatus
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public IntegrationStatus()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class IntegrationType
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public IntegrationType()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class IntegrationError
{
    [Key]
    public int Id { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }

    public DateTime ErrorTime { get; set; }

    public bool Deleted { get; set; }

    public IntegrationError()
    {
        Id = 0;
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
        ErrorTime = DateTime.Now;
        Deleted = false;

    }

}

public class ProjectNote
{
    [Key]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Note { get; set; }

    public bool Deleted { get; set; }

    public ProjectNote()
    {
        Id = 0;
        ProjectId = 0;
        Note = string.Empty;
        Deleted = false;

    }

}

public class Severity
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public Severity()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class SupportStatus
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public SupportStatus()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class Ticket
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int IntegrationId { get; set; }

    public int SeverityId { get; set; }

    public int? EscalationId { get; set; }

    public string? Reproduce { get; set; }

    public string ReportedBy { get; set; }

    public string? AssignedTo { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ResolutionDate { get; set; }

    public string? Resolution { get; set; }

    public bool Deleted { get; set; }

    public Ticket()
    {
        Id = 0;
        CustomerId = 0;
        IntegrationId = 0;
        SeverityId = 0;
        EscalationId = null;
        Reproduce = string.Empty;
        ReportedBy = string.Empty;
        AssignedTo = string.Empty;
        Description = string.Empty;
        CreatedDate = DateTime.Now;
        ResolutionDate = null;
        Resolution = string.Empty;
        Deleted = false;

    }

}

public class Escalation
{
    [Key]
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string ProblemSummary { get; set; }

    public string? CustomerImpact { get; set; }

    public string? RootCause { get; set; }

    public string? RecommendedAction { get; set; }

    public bool Deleted { get; set; }

    public Escalation()
    {
        Id = 0;
        TicketId = 0;
        ProblemSummary = string.Empty;
        CustomerImpact = string.Empty;
        RootCause = string.Empty;
        RecommendedAction = string.Empty;
        Deleted = false;

    }

}

public class TicketNote
{
    [Key]
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string Note { get; set; }

    public bool Deleted { get; set; }

    public TicketNote()
    {
        Id = 0;
        TicketId = 0;
        Note = string.Empty;
        Deleted = false;

    }

}
