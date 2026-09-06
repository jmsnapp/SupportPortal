CREATE PROCEDURE [dbo].[uspGetAllIntegrations]
AS
	SELECT [dbo].[Integrations].[Id]
	, [dbo].[Integrations].[Name]
	, [dbo].[Integrations].[Description]
	, [dbo].[Integrations].[Deleted]
	, [dbo].[Integrations].[RowVersion]
	, [dbo].[Integrations].[CustomerId]
	, [dbo].[Integrations].[IntegrationTypeId]
	, [dbo].[Integrations].[CurrentStatusId]
	, [dbo].[Integrations].[LastSuccessfulSync]
	, [dbo].[Integrations].[LastFailedSync]
	, [dbo].[Integrations].[RetryCount] 
	, [dbo].[Customers].[Description] AS 'CustomerDescription'
	, [dbo].[IntegrationTypes].[Description] AS 'IntegrationTypeDescription'
	, [dbo].[IntegrationStatuses].[Description] AS 'CurrentStatusDescription'
	FROM [dbo].[Integrations]
		LEFT JOIN [dbo].[IntegrationTypes] ON [dbo].[Integrations].[IntegrationTypeId] = [dbo].[IntegrationTypes].[Id]
		LEFT JOIN [dbo].[IntegrationStatuses] ON [dbo].[Integrations].[CurrentStatusId] = [dbo].[IntegrationStatuses].[Id]
		LEFT JOIN [dbo].[Customers] ON [dbo].[Integrations].[CustomerId] = [dbo].[Customers].[Id]
