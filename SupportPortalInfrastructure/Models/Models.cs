using System;
using System.ComponentModel.DataAnnotations;

namespace SupportPortalInfrastructure.Models;

// Minimal placeholder POCOs for scaffolding target. Replace with scaffolded classes when you reverse-engineer the DB.

public class Industry
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Customer
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Project
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Phase
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class LinkProjectPhase
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int PhaseId { get; set; }
}

public class Integration
{
    [Key]
    public int Id { get; set; }
    public string? Description { get; set; }
}

public class IntegrationStatus
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class IntegrationType
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class IntegrationError
{
    [Key]
    public int Id { get; set; }
    public string? Message { get; set; }
    public DateTime? OccurredAt { get; set; }
}

public class ProjectNote
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? Note { get; set; }
}

public class Severity
{
    [Key]
    public int Id { get; set; }
    public string? Level { get; set; }
}

public class SupportStatus
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Ticket
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Escalation
{
    [Key]
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string? Reason { get; set; }
}

public class TicketNote
{
    [Key]
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string? Note { get; set; }
}