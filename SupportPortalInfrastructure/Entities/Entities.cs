using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalInfrastructure.Entities;

public class PortalEntity
{
    [Key]
    public Int64 Id { get; set; }

    [StringLength(63), Required]
    public string Name { get; set; }

    [StringLength(255), Required]
    public virtual string Description { get; set; }

    [Required]
    public bool Deleted { get; set; }

    /// <summary>
    /// Optimistic concurrency token, maintained by SQL Server. The only meaningful value a
    /// caller can supply is the one it was handed on read - see GenericRepository.Update.
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public PortalEntity()
    {
        Id = -1;
        Name = string.Empty;
        Description = string.Empty;
        Deleted = false;

    }

}

public class CustomerEntity : PortalEntity
{
    [Required]
    public Int64 IndustryId { get; set; }

    [StringLength(63), Required]
    public string PrimaryContactName { get; set; }

    [StringLength(63), Required, EmailAddress]
    public string PrimaryContactEmail { get; set; }

    [StringLength(63), Required]
    public string TechnicalContactName { get; set; }

    [StringLength(63), Required, EmailAddress]
    public string TechnicalContactEmail { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public IndustryEntity? Industry { get; set;  }

    public CustomerEntity()
    {
        IndustryId = 0;
        PrimaryContactName = string.Empty;
        PrimaryContactEmail = string.Empty;
        TechnicalContactName = string.Empty;
        TechnicalContactEmail = string.Empty;
        CreatedDate = DateTime.UtcNow;

    }

}

public class EscalationEntity  : PortalEntity
{
    [Required]
    public string ProblemSummary { get; set; }

    [Required]
    public string CustomerImpact { get; set; }

    public string RootCause { get; set; }

    public string RecommendedActions { get; set; }

    public DateTime CreatedDate { get; set; }

    public EscalationEntity()
    {
        ProblemSummary = string.Empty;
        CustomerImpact = string.Empty;
        RootCause = string.Empty;
        RecommendedActions = string.Empty;
        CreatedDate = DateTime.UtcNow;

    }

}

public class IndustryEntity : PortalEntity
{ }

public class IntegrationEntity : PortalEntity
{
    [StringLength(127)]
    public override string Description { get; set; }

    [Required]
    public Int64 CustomerId { get; set; }

    [Required]
    public Int64 IntegrationTypeId { get; set; }

    [Required]
    public Int64 CurrentStatusId { get; set; }

    public DateTime LastSuccessfulSync { get; set; }

    public DateTime LastFailedSync { get; set; }

    public int RetryCount { get; set; }

    public CustomerEntity? Customer { get; set; }

    public IntegrationTypeEntity? IntegrationType { get; set; }

    public IntegrationStatusEntity? CurrentStatus { get; set; }

    public IntegrationEntity()
    {
        Description = string.Empty;
        CustomerId = 0;
        IntegrationTypeId = 0;
        CurrentStatusId = 0;
        LastSuccessfulSync = new DateTime(1900, 1, 1);
        LastFailedSync = new DateTime(1900, 1, 1);
        RetryCount = 0;

    }

}

public class IntegrationErrorEntity : PortalEntity
{
    [Required]
    public Int64 IntegrationId { get; set; }

    [Required, StringLength(1027)]
    public string ErrorMessage { get; set; }

    public string StackTrace { get; set; }

    [Required]
    public DateTime ErrorTime { get; set; }

    public IntegrationEntity? Integration {  get; set; }

    public IntegrationErrorEntity()
    {
        IntegrationId = 0;
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
        ErrorTime = DateTime.UtcNow;

    }

}

public class IntegrationStatusEntity : PortalEntity
{ }

public class IntegrationTypeEntity : PortalEntity
{ }

public class LinkProjectPhaseEntity : PortalEntity
{
    [Required]
    public Int64 ProjectId { get; set; }

    [Required]
    public Int64 PhaseId { get; set; }

    public decimal Percentage { get; set; }

    public int Order { get; set; }

    public ProjectEntity? Project { get; set; }

    public PhaseEntity? Phase { get; set; }

    public LinkProjectPhaseEntity()
    {
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
    [Required]
    public Int64 CustomerId { get; set; }

    [Required]
    public Int64 CurrentPhaseId { get; set; }

    [Required]
    public DateTime TargetGoLiveDate { get; set; }

    public DateTime ActualGoLiveDate { get; set; }

    public CustomerEntity? Customer { get; set; }

    public PhaseEntity? CurrentPhase { get; set; }

    public List<LinkProjectPhaseEntity> Phases { get; set; }

    public List<ProjectNoteEntity> Notes { get; set; }

    public ProjectEntity()
    {
        CustomerId = 0;
        CurrentPhaseId = 0;
        Phases = new List<LinkProjectPhaseEntity>();
        Notes = new List<ProjectNoteEntity>();
        TargetGoLiveDate = DateTime.UtcNow;
        ActualGoLiveDate = new DateTime(1900, 1, 1);

    }

}

public class ProjectNoteEntity : PortalEntity
{
    [Required]
    public Int64 ProjectId { get; set; }

    [Required]
    public string Note { get; set; }

    public DateTime CreateTime { get; set; }

    public ProjectEntity? Project { get; set; }

    public ProjectNoteEntity()
    {
        ProjectId = 0;
        Note = string.Empty;
        CreateTime = DateTime.UtcNow;

    }

}

public class SeverityEntity : PortalEntity
{ }

public class SupportStatusEntity : PortalEntity
{ }

public class TicketEntity : PortalEntity
{

    [StringLength(1023)]
    public override string Description { get; set; }

    [Required]
    public Int64 CustomerId { get; set; }

    [Required]
    public Int64 IntegrationId { get; set; }

    [Required]
    public Int64 SeverityId { get; set; }

    public Int64 EscalationId { get; set; }

    [Required]
    public Int64 StatusId { get; set; }

    public string Reproduce { get; set; }

    [Required, StringLength(63)]
    public string ReportedBy { get; set; }

    [StringLength(63)]
    public string AssignedTo { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime ResolutionDate { get; set; }

    public string Resolution { get; set; }

    public CustomerEntity? Customer { get; set; }

    public IntegrationEntity? Integration { get; set; }

    public SeverityEntity? Severity { get; set; }

    public EscalationEntity? Escalation { get; set; }

    public SupportStatusEntity? Status { get; set; }

    public List<TicketNoteEntity> Notes { get; set; }

    public TicketEntity()
    {
        Description = string.Empty;
        CustomerId = 0;
        IntegrationId = 0;
        SeverityId = 0;
        StatusId = 0;
        EscalationId = 0;
        Notes = new List<TicketNoteEntity>();
        Reproduce = string.Empty;
        ReportedBy = string.Empty;
        AssignedTo = string.Empty;
        CreatedDate = DateTime.UtcNow;
        ResolutionDate = new DateTime(1900, 1, 1);
        Resolution = string.Empty;

    }

}

public class TicketNoteEntity : PortalEntity
{
    [Required]
    public Int64 TicketId { get; set; }

    [Required]
    public string Note { get; set; }

    public DateTime CreateTime { get; set; }

    public TicketEntity? Ticket { get; set; }

    public TicketNoteEntity()
    {
        TicketId = 0;
        Note = string.Empty;
        CreateTime = DateTime.UtcNow;

    }

}
