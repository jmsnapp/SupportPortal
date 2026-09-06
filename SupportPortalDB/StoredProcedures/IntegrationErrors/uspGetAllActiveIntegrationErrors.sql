CREATE PROCEDURE [dbo].[uspGetAllActiveIntegrationErrors]
AS
	SELECT [dbo].[IntegrationErrors].[Id]
	, [dbo].[IntegrationErrors].[Description]
	, [dbo].[IntegrationErrors].[Deleted]
	, [dbo].[IntegrationErrors].[RowVersion]
	, [dbo].[IntegrationErrors].[IntegrationId]
	, [dbo].[IntegrationErrors].[ErrorMessage]
	, [dbo].[IntegrationErrors].[StackTrace]
	, [dbo].[IntegrationErrors].[ErrorTime] 
	, [dbo].[Integrations].[Description] AS 'IntegrationDescription'
	FROM [dbo].[IntegrationErrors]
		LEFT JOIN [dbo].[Integrations] ON [dbo].[IntegrationErrors].[IntegrationId] = [dbo].[Integrations].[Id]
	WHERE [dbo].[IntegrationErrors].[Deleted] = 0
