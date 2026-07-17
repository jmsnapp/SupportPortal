Project Name

SupportPortal
Customer Implementation & Support Portal

Tagline for GitHub:

    A .NET SaaS-style support engineering demo for customer onboarding, implementation tracking, integration troubleshooting, SQL-backed reporting, and engineering escalation workflows.

Recommended Tech Stack

    .NET 10 / ASP.NET Core Web API for the backend. .NET 10 is currently listed as Long Term Support through November 2028, and ASP.NET Core is Microsoft’s framework for building fast, secure, cross-platform web apps and services. 
    MSSQL Server for the Database. 
    Entity Framework Core migrations for schema management. EF Core supports schema migrations and Microsoft specifically notes migration scripts can be reviewed and used in deployment workflows. 
    GitHub Actions for build/test CI. GitHub has official guidance for building and testing .NET projects using Actions. 
    OpenTelemetry or structured logging as a stretch feature. Microsoft’s .NET observability docs describe OpenTelemetry support for logs, metrics, and traces. 

For UI:

    Blazor
    No fancy frontend at first — Swagger/OpenAPI plus a clean README is enough for MVP.

The Product Concept

SupportPortal helps a fictional B2B SaaS company manage customer implementations and support escalations.

The company sells software to customers who need integrations. Customers go through onboarding, connect external systems, encounter issues, and sometimes require engineering investigation.

So the app tracks:

    Customers
    Implementation projects
    Integration configurations
    Support tickets
    Root cause investigations
    Engineering escalations
    Customer launch readiness
    Support metrics

MVP Scope
User Personas
Implementation Specialist

Owns onboarding and launch readiness.

Needs to answer:

    Which customers are blocked?
    What integrations are incomplete?
    What risks exist before go-live?
    What actions are needed this week?

Support Engineer

Investigates issues and documents root cause.

Needs to answer:

    What is the reported problem?
    Can I reproduce it?
    What data or logs point to the cause?
    Does this need engineering escalation?

Engineering Team

Receives clean, detailed escalation notes.

Needs to answer:

    What happened?
    Steps to reproduce?
    Actual vs expected behavior?
    Relevant customer config/data?
    Suggested fix or suspected component?

Customer Success Manager

Tracks customer health and adoption.

Needs to answer:

    Is the customer live?
    Are they blocked?
    Are repeated issues emerging?
    Is time-to-value improving?

Core Features
1. Customer Management

Each customer should have:

    Name
    Industry
    Status: Prospect, Onboarding, Live, At Risk, Churned
    Primary contact
    Technical contact
    Contract start date
    Target go-live date
    Notes

Example customers:

    “Acme Rehab Group”
    “Northstar Logistics”
    “Bluebird Marketplace”
    “Franklin Therapy Partners”

2. Implementation Projects

Each customer can have one or more onboarding projects.

Fields:

    CustomerId
    Project name
    Implementation owner
    Current phase
    Target go-live date
    Actual go-live date
    Risk level
    Blocker summary
    Percent complete

Phases:

    Discovery
    Configuration
    Integration Setup
    Data Validation
    Training
    Launch
    Post-Launch Support

3. Integration Tracker

Each customer can have integrations like:

    Orders API
    Claims API
    Webhook endpoint
    SFTP import
    Carrier API
    Reporting export

Fields:

    Integration type
    Provider name
    Status
    Last successful sync
    Last failed sync
    Error code
    Error message
    Retry count
    Configuration notes

4. Support Tickets

Fields:

    Customer
    Title
    Description
    Severity
    Status
    Created date
    Assigned to
    Reported by
    Steps to reproduce
    Expected behavior
    Actual behavior
    Root cause category
    Resolution summary
    Engineering escalation needed: yes/no

Severity:

    Low
    Medium
    High
    Critical

Status:

    New
    Investigating
    Waiting on Customer
    Escalated to Engineering
    Resolved
    Closed

5. Root Cause Investigation Notes

Each ticket should have investigation notes:

    Timestamp
    Investigator
    Observation
    SQL query used
    Result summary
    Next action

Example:

    Checked failed webhook deliveries for Customer 104. Found repeated HTTP 401 responses beginning after API key rotation. Confirmed stored key hash does not match current customer configuration.

6. Engineering Escalation Generator

Add a button or endpoint that creates an “engineering handoff” summary from a support ticket:

    Problem summary
    Customer impact
    Reproduction steps
    Logs/data checked
    Suspected root cause
    Relevant SQL query
    Suggested next action

Even if it just generates Markdown, that is great.
7. Dashboard / Reports

Keep the dashboard simple:

    Open tickets by severity
    Customers blocked from go-live
    Average time to resolution
    Tickets by root cause category
    Integrations with repeated failures
    Implementations due in the next 14 days

Suggested Architecture

Keep it clean and enterprise-looking, not overengineered.

SupportPortal
│
├── SupportPortal.Api
│   ├── Controllers / Endpoints
│   ├── DTOs
│   ├── Validation
│   └── Swagger/OpenAPI
│
├── SupportPortal.Application
│   ├── Services
│   ├── Use Cases
│   ├── Interfaces
│   └── Business Rules
│
├── SupportPortal.Domain
│   ├── Entities
│   ├── Enums
│   └── Domain Models
│
├── SupportPortal.Infrastructure
│   ├── EF Core DbContext
│   ├── Repositories
│   ├── Database Migrations
│   ├── Seed Data
│   └── External Integration Simulators
│
├── SupportPortal.Tests
│   ├── Unit Tests
│   └── Integration Tests
│
└── docs
    ├── implementation-guide.md
    ├── troubleshooting-runbook.md
    ├── engineering-escalation-template.md
    └── sample-root-cause-writeups.md


Minimum tables:

Customers
- Id
- Name
- Industry
- Status
- PrimaryContactName
- PrimaryContactEmail
- TechnicalContactName
- TechnicalContactEmail
- CreatedAt

ImplementationProjects
- Id
- CustomerId
- ProjectName
- Phase
- RiskLevel
- TargetGoLiveDate
- ActualGoLiveDate
- PercentComplete
- BlockerSummary

ImplementationTasks
- Id
- ProjectId
- Title
- Description
- Status
- DueDate
- CompletedAt
- Owner

Integrations
- Id
- CustomerId
- IntegrationType
- ProviderName
- Status
- LastSuccessfulSync
- LastFailedSync
- ErrorCode
- ErrorMessage
- RetryCount

SupportTickets
- Id
- CustomerId
- Title
- Description
- Severity
- Status
- ReportedBy
- AssignedTo
- CreatedAt
- ResolvedAt
- ExpectedBehavior
- ActualBehavior
- RootCauseCategory
- ResolutionSummary
- EngineeringEscalationNeeded

InvestigationNotes
- Id
- TicketId
- CreatedAt
- Investigator
- Observation
- SqlQueryUsed
- ResultSummary
- NextAction

EngineeringEscalations
- Id
- TicketId
- CreatedAt
- ProblemSummary
- CustomerImpact
- StepsToReproduce
- SuspectedRootCause
- TechnicalDetails
- RecommendedAction

Stretch tables:

WebhookEvents
ApiRequestLogs
CustomerHealthScores
KnowledgeBaseArticles
ReleaseNotes

API Endpoints
Customers

GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
GET    /api/customers/{id}/health

Implementations

GET    /api/implementations
GET    /api/implementations/blocked
GET    /api/implementations/due-soon
POST   /api/implementations
PUT    /api/implementations/{id}/phase
POST   /api/implementations/{id}/tasks
PUT    /api/implementation-tasks/{id}/complete

Integrations

GET    /api/integrations
GET    /api/integrations/failing
POST   /api/integrations/{id}/simulate-failure
POST   /api/integrations/{id}/retry

Support Tickets

GET    /api/tickets
GET    /api/tickets/{id}
POST   /api/tickets
PUT    /api/tickets/{id}/status
POST   /api/tickets/{id}/investigation-notes
POST   /api/tickets/{id}/generate-escalation

Reports

GET    /api/reports/open-tickets-by-severity
GET    /api/reports/time-to-resolution
GET    /api/reports/root-cause-categories
GET    /api/reports/blocked-customers

Three Built-In Demo Scenarios

These should be seeded into the database so an evaluator can clone the app and immediately see useful examples.
Scenario 1: Failed Webhook After API Key Rotation

Customer complaint: “Orders stopped syncing yesterday.”

Symptoms:

    Webhook failures began at 2:14 PM.
    Error code: 401 Unauthorized.
    Retry count increased.
    Last successful sync occurred before customer rotated API key.

Root cause:

Customer updated API key in external system but not in SupportPortal configuration.

Resolution:

Update stored API credential, retry failed webhook events, document prevention step.

Skills shown:

    API troubleshooting
    Customer communication
    Root cause analysis
    Configuration investigation

Scenario 2: Duplicate Order Import

Customer complaint: “We are seeing duplicate orders in the platform.”

Symptoms:

    Same external order ID appears multiple times.
    Duplicate records entered after retry logic ran.
    No uniqueness constraint on customer/external order ID.

Root cause:

Retry process was not idempotent.

Resolution:

Add validation check before insert, recommend unique index, document engineering escalation.

Skills shown:

    SQL investigation
    Data integrity
    Engineering handoff
    Production issue prevention

Scenario 3: Customer Blocked Before Go-Live

Customer complaint: “We cannot launch Monday because imported order totals do not match our source system.”

Symptoms:

    2.8% of imported orders missing shipping adjustment.
    Issue isolated to one carrier mapping.
    Configuration incomplete for one provider.

Root cause:

Carrier service code mapping missing for a specific shipping method.

Resolution:

Update mapping table, rerun validation report, mark launch blocker resolved.

Skills shown:

    Implementation support
    SQL validation
    Customer onboarding
    Risk tracking

Week-by-Week Build Plan
This Weekend: Project Foundation

Goal: have a real repo, real structure, real database model, and a README.
Saturday

    Create GitHub repo: SupportPortal
    Add README with:
        Business problem
        Target roles
        Tech stack
        Feature list
        Architecture diagram
        Build/run instructions
    Create solution/projects:
        SupportPortal.Api
        SupportPortal.Application
        SupportPortal.Domain
        SupportPortal.Infrastructure
        SupportPortal.Tests
    Add core entities:
        Customer
        ImplementationProject
        Integration
        SupportTicket
        InvestigationNote
    Add enums:
        CustomerStatus
        TicketSeverity
        TicketStatus
        ImplementationPhase
        IntegrationStatus

Sunday

    Configure MSSQL.
    Add EF Core DbContext.
    Create first migration.
    Seed 4 fake customers.
    Add first API endpoints:
        GET /api/customers
        GET /api/tickets
        POST /api/tickets
    Add Swagger/OpenAPI.
    Add 3 screenshots to README, even if basic.

Weekend deliverable:
An evaluator can open the repo and see a real project with a professional purpose.

Week 1: Ticketing + Customer Support Workflow

Goal: make the support workflow credible.

Build:

    Customer CRUD
    Ticket CRUD
    Ticket status changes
    Investigation notes
    Ticket filtering by severity/status
    Seeded demo tickets

Add documentation:

    docs/troubleshooting-runbook.md
    docs/sample-root-cause-writeups.md

Add at least 3 unit tests:

    Create ticket
    Add investigation note
    Change status to resolved

End of Week 1 deliverable:
A working support-ticket system with root-cause notes.

Week 2: Implementation / Onboarding Module

Goal: make it relevant to Solutions Engineer and Implementation Consultant roles.

Build:

    Implementation project CRUD
    Implementation task checklist
    Phase changes
    Risk level tracking
    Blocked customer report
    Due-soon implementation report

Add seeded data:

    One customer in Discovery
    One in Integration Setup
    One blocked before launch
    One successfully launched

Add documentation:

    docs/customer-onboarding-guide.md
    docs/implementation-checklist.md

End of Week 2 deliverable:
A working implementation tracker that looks like something a SaaS company would actually use.

Week 3: Integration Simulator + Troubleshooting

Goal: show technical depth.

Build:

    Integration records
    Simulated webhook failures
    Simulated API sync failure
    Simulated duplicate import issue
    Retry endpoint
    Failing integrations report

Add 2–3 SQL scripts:

/sql/find-failed-integrations.sql
/sql/duplicate-order-investigation.sql
/sql/blocked-implementations.sql

Add documentation:

    docs/integration-troubleshooting-guide.md
    docs/engineering-escalation-template.md

End of Week 3 deliverable:
This is where the project becomes a true Support Engineer / Solutions Engineer portfolio piece.

Week 4: Polish, CI, Observability, Recruiter Packaging

Goal: make it look finished and professional.

Add:

    GitHub Actions build/test workflow
    Better README
    Screenshots
    Architecture diagram
    Sample demo script
    Logging
    Optional OpenTelemetry basics
    More tests
    “Known limitations / future improvements” section

Create:

    docs/demo-script.md
    docs/architecture.md
    docs/support-engineering-case-study.md

End of Week 4 deliverable:
A polished repo.

README Structure

# SupportPortal

## Overview
Short business description.

## Why This Project Exists
Explain the customer support / implementation problem.

## Features
- Customer onboarding tracking
- Support ticket triage
- Integration failure simulation
- Root cause investigation notes
- Engineering escalation summaries
- SQL-backed reporting

## Tech Stack
- .NET 10
- ASP.NET Core Web API
- PostgreSQL
- EF Core
- GitHub Actions
- OpenTelemetry / structured logging

## Architecture
Include diagram.

## Demo Scenarios
1. Failed webhook after API key rotation
2. Duplicate order import
3. Blocked customer go-live

## Screenshots
Add screenshots.

## How to Run Locally
Step-by-step instructions.

## Sample SQL Investigations
Show a few queries.

## What This Demonstrates
Tie directly to target jobs.

## Future Enhancements
Show judgment without overbuilding.

Build it in this order:

    README first.
    Database model second.
    API third.
    Demo scenarios fourth.
    Documentation fifth.
    UI last.
