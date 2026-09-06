CREATE PROCEDURE [dbo].[uspGetAllTickets]
AS
	SELECT [dbo].[Tickets].[Id]
	, [dbo].[Tickets].[Description]
	, [dbo].[Tickets].[Deleted]
	, [dbo].[Tickets].[RowVersion]
	, [dbo].[Tickets].[CustomerId]
	, [dbo].[Tickets].[IntegrationId]
	, [dbo].[Tickets].[Reproduce]
	, [dbo].[Tickets].[SeverityId]
	, [dbo].[Tickets].[StatusId]
	, [dbo].[Tickets].[ReportedBy]
	, [dbo].[Tickets].[AssignedTo]
	, [dbo].[Tickets].[CreatedDate]
	, [dbo].[Tickets].[ResolutionDate]
	, [dbo].[Tickets].[Resolution]
	, [dbo].[Tickets].[EscalationId]
	, [dbo].[Customers].[Description] AS 'CustomerDescription'
	, [dbo].[Integrations].[Description] AS 'IntegrationDescription'
	, [dbo].[Severities].[Description] AS 'SeverityDescription'
	, [dbo].[SupportStatuses].[Description] AS 'StatusDescription'
	, [dbo].[Escalations].[Description] AS 'EscalationDescription'
	FROM [dbo].[Tickets]
		LEFT JOIN [dbo].[Customers] ON [dbo].[Tickets].[CustomerId] = [dbo].[Customers].[Id]
		LEFT JOIN [dbo].[Integrations] ON [dbo].[Tickets].[IntegrationId] = [dbo].[Integrations].[Id]
		LEFT JOIN [dbo].[Severities] ON [dbo].[Tickets].[SeverityId] = [dbo].[Severities].[Id]
		LEFT JOIN [dbo].[SupportStatuses] ON [dbo].[Tickets].[StatusId] = [dbo].[SupportStatuses].[Id]
		LEFT JOIN [dbo].[Escalations] ON [dbo].[Tickets].[EscalationId] = [dbo].[Escalations].[Id]
