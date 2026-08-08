using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalInfrastructure.Entities;

public class PortalEntity
{
    [Key]
    public Int64 Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; }

    public PortalEntity()
    {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class CustomerEntity : PortalEntity
{
    public Int64 IndustryId { get; set; }

    public string PrimaryContactName { get; set; }

    public string PrimaryContactEmail { get; set; }

    public string TechnicalContactName { get; set; }

    public string TechnicalContactEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    public CustomerEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        IndustryId = 0;
        PrimaryContactName = string.Empty;
        PrimaryContactEmail = string.Empty;
        TechnicalContactName = string.Empty;
        TechnicalContactEmail = string.Empty;
        CreatedDate = DateTime.Now;

    }

}

public class EscalationEntity  : PortalEntity
{
    public string ProblemSummary { get; set; }

    public string? CustomerImpact { get; set; }

    public string? RootCause { get; set; }

    public string? RecommendedActions { get; set; }

    public EscalationEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        ProblemSummary = string.Empty;
        CustomerImpact = string.Empty;
        RootCause = string.Empty;
        RecommendedActions = string.Empty;

    }

}

public class IndustryEntity : PortalEntity
{ }

public class IntegrationEntity : PortalEntity
{
    public Int64 CustomerId { get; set; }

    public Int64 IntegrationTypeId { get; set; }

    public Int64 CurrentStatusId { get; set; }

    public DateTime? LastSuccessfulSync { get; set; }

    public DateTime? LastFailedSync { get; set; }

    public int RetryCount { get; set; }

    public IntegrationEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        CustomerId = 0;
        IntegrationTypeId = 0;
        CurrentStatusId = 0;
        LastSuccessfulSync = null;
        LastFailedSync = null;
        RetryCount = 0;

    }

}

public class IntegrationErrorEntity : PortalEntity
{
    public Int64 IntegrationId { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }

    public DateTime ErrorTime { get; set; }

    public IntegrationErrorEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        IntegrationId = 0;
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
        ErrorTime = DateTime.Now;

    }

}

public class IntegrationStatusEntity : PortalEntity
{ }

public class IntegrationTypeEntity : PortalEntity
{ }

public class LinkProjectPhaseEntity : PortalEntity
{
    public Int64 ProjectId { get; set; }

    public Int64 PhaseId { get; set; }

    public decimal Percentage { get; set; }

    public int Order { get; set; }

    public LinkProjectPhaseEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        ProjectId = 0;
        PhaseId = 0;
        Percentage = 0;
        Order = 0;

    }

}

public class PhaseEntity : PortalEntity
{ }

public class ProjectEntity : PortalEntity
{
    public Int64 CurrentPhase { get; set; }

    public DateTime TargetGoLiveDate { get; set; }

    public DateTime? ActualGoLiveDate { get; set; }

    public ProjectEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        CurrentPhase = 0;
        TargetGoLiveDate = DateTime.Now;

    }

}

public class ProjectNoteEntity : PortalEntity
{
    public Int64 ProjectId { get; set; }

    public string Note { get; set; }

    public ProjectNoteEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        ProjectId = 0;
        Note = string.Empty;

    }

}

public class SeverityEntity : PortalEntity
{ }

public class SupportStatusEntity : PortalEntity
{ }

public class TicketEntity : PortalEntity
{

    public Int64 CustomerId { get; set; }

    public Int64 IntegrationId { get; set; }

    public Int64 SeverityId { get; set; }

    public Int64? EscalationId { get; set; }

    public Int64 StatusId { get; set; }

    public string? Reproduce { get; set; }

    public string ReportedBy { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ResolutionDate { get; set; }

    public string? Resolution { get; set; }

    public TicketEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        CustomerId = 0;
        IntegrationId = 0;
        SeverityId = 0;
        StatusId = 0;
        EscalationId = null;
        Reproduce = string.Empty;
        ReportedBy = string.Empty;
        AssignedTo = string.Empty;
        CreatedDate = DateTime.Now;
        ResolutionDate = null;
        Resolution = string.Empty;

    }

}

public class TicketNoteEntity : PortalEntity
{
    public Int64 TicketId { get; set; }

    public string Note { get; set; }

    public TicketNoteEntity()
    {
        base.Id = 0;
        base.Name = string.Empty;
        base.Description = string.Empty;
        base.Deleted = false;

        TicketId = 0;
        Note = string.Empty;

    }

}
