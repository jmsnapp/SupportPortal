# SupportPortalUI — Functional Specification

Version: 1.0  
Date: 2026-08-06

## Overview
SupportPortalUI is a server-side Blazor UI running on .NET 10. It consumes the SupportPortalAPI over HTTP (unauthenticated in this phase) and follows an MVC-style separation in the UI (Models, Service/Controller clients, Razor component Views).

The main page shows, in this order (top → bottom):
1. Escalations
2. Open Tickets
3. Open Projects
4. Active Integrations

Supporting-object maintenance UIs and Notes/Phases CTAs are included.

## Goals
- Provide an operational dashboard surfacing current escalations, support tickets, implementation projects, and integrations.
- Allow lightweight maintenance of supporting domain objects (customers, industries, statuses, types, phases, severities).
- Provide CTAs for creating and navigating relationships:
  - Ticket → Create Escalation
  - Ticket → Show Ticket Notes
  - Project → Show Project Notes
  - Project → Show / Manage Project Phases
  - Escalation → List linked Tickets

Authentication/authorization is intentionally deferred to a later phase. API calls are unauthenticated for now.

## High-level Architecture
- UI: Server-side Blazor (.NET 10)
- Data access: Typed `HttpClient` ApiClients calling `SupportPortalAPI` endpoints (e.g., `GET api/escalations/active`, `POST api/escalations`, `PUT api/tickets/{id}`).
- Controllers/Services in UI: lightweight API clients (e.g., `IEscalationsApiClient`, `ITicketsApiClient`, `IProjectsApiClient`, `IIntegrationsApiClient`, `ICustomersApiClient`, etc.).
- Views: Razor Components (`Pages/*`) and shared components.

## Main Page (Home)
- Route: `/`
- Sections (each independent with own loading state and retry):
  1. Escalations
     - Source: `GET /api/escalations/active`
     - Show top N (configurable, default 5).
     - Columns/summary: `Id`, `Name` (link to `/escalations/{id}`), truncated `ProblemSummary`, `CreatedDate`.
     - Highlight escalations with non-empty `CustomerImpact`.
     - Actions: View details.
  2. Open Tickets
     - Source: `GET /api/tickets/active` (filter: not deleted, status != closed).
     - Show top N (default 10), sorted by `CreatedDate DESC`.
     - Columns: `Id`, `Name` (link), `Severity`, `Status`, `AssignedTo`, `CreatedDate`.
     - Actions: View details, inline quick status change (calls `PUT /api/tickets/{id}`).
     - Additional CTA: open `Ticket Notes` and `Create Escalation`.
  3. Open Projects
     - Source: `GET /api/projects/active`
     - Show top N (default 8), sorted by `TargetGoLiveDate ASC`.
     - Columns: `Id`, `Name` (link), `CurrentPhase`, `TargetGoLiveDate`.
     - CTA: View Project Notes, Manage Project Phases.
  4. Active Integrations
     - Source: `GET /api/integrations/active`
     - Show Name (link), `IntegrationType`, `CurrentStatus`, `LastSuccessfulSync`, `RetryCount`.
     - Highlight unhealthy integrations.

## Escalation & Ticket Interactions
- Create Escalation from Ticket:
  - UI flow on `/tickets/{id}`: "Create Escalation" CTA opens modal or prefilled creation page.
  - Sequence: `POST /api/escalations` → on success, `PUT /api/tickets/{ticketId}` setting `EscalationId` to new escalation `Id`.
  - After success, navigate to `/escalations/{id}`.
  - Failure handling: surface errors and allow retry or rollback guidance.
- Escalation Detail:
  - `GET /api/escalations/{id}`
  - List linked tickets where `ticket.EscalationId == escalation.Id`.
  - Preferred optimization later: `GET /api/tickets?escalationId={id}` server-side filtering.

## Notes & Project Phases
- Ticket Notes:
  - Endpoint: `api/ticketnotes`
  - UI: `/tickets/{id}/notes` (or modal). List notes (Author, Text, CreatedDate), create new note (`POST`).
- Project Notes:
  - Endpoint: `api/projectnotes`
  - UI: `/projects/{id}/notes`. Same UX as ticket notes.
- Project Phases:
  - Entity: `LinkProjectPhase` via `api/linkprojectphases`.
  - UI: `/projects/{id}/phases`. List linked phases, allow add (select from `GET api/phases/getall` then `POST link`) and remove (delete or set Deleted=true via `PUT`).

## Supporting Objects Maintenance
- Admin pages (list + create/edit/delete) for:
  - Customers (`/admin/customers`)
  - Industries (`/admin/industries`)
  - Integration Statuses (`/admin/integration-statuses`)
  - Integration Types (`/admin/integration-types`)
  - Phases (`/admin/phases`)
  - Severities (`/admin/severities`)
  - Support Statuses (`/admin/support-statuses`)
- Usage: call the API controllers (`GET getall`, `GET {id}`, `POST`, `PUT {id}`).
- Soft-delete: toggle `Deleted` flag via `PUT`.

## ApiClients & DI
- Add typed ApiClients (register in `Program.cs`):
  - `IEscalationsApiClient`, `ITicketsApiClient`, `IProjectsApiClient`, `IIntegrationsApiClient`
  - `ICustomersApiClient`, `IIndustriesApiClient`, `IIntegrationStatusesApiClient`, `IIntegrationTypesApiClient`
  - `IPhasesApiClient`, `ISeveritiesApiClient`, `ISupportStatusesApiClient`
  - `ITicketNotesApiClient`, `IProjectNotesApiClient`, `ILinkProjectPhasesApiClient`
- Use `builder.Services.AddHttpClient<TInterface, TImpl>(c => c.BaseAddress = new Uri(config["SupportPortalApi:BaseUrl"]))`.
- Include transient-fault handling (Polly) but no auth handlers.

## UX / Accessibility / Error Handling
- Each section shows spinner while loading and an inline error block on failure.
- Use Bootstrap (already referenced). Use semantic headings and focus management (`FocusOnNavigate`).
- Provide confirmation for destructive actions.
- No UI role gating in this phase.

## Performance & Scalability
- Home page queries should limit returned records (server-side paging). If API lacks paging, UI enforces `take` and may fetch `getall` then trim.
- Later improvement: add server-side filter endpoints for notes, linked objects, and tickets by escalation/project.

## Acceptance Criteria
- Visiting `/` displays Escalations, Tickets, Projects, Integrations in required order.
- Ticket detail page has CTAs for "Create Escalation" and "Show Notes".
- Escalation detail lists linked tickets with links to ticket details.
- Project detail page has CTAs for "Show Project Notes" and "Manage Phases".
- Admin pages exist for all supporting objects and can Create/Edit/Delete via API.
- All API calls are unauthenticated in this phase and UI surfaces loading/errors.

## Test Plan (high level)
- Unit tests for ApiClients (mock HttpMessageHandler).
- Component tests for Home, Ticket detail (notes & create escalation), Escalation detail, Project phases/modal.
- Integration tests using a test instance of SupportPortalAPI (or mocked endpoints).

## Future Improvements
- Add authentication/authorization and UI role gating.
- Server-side endpoints supporting query-by-parent (e.g., tickets?escalationId=).
- Atomic server-side operations for create-escalation-and-link to avoid orphaned entities.
- Real-time updates (SignalR) for escalations/tickets/integrations.
