CREATE PROCEDURE [dbo].[uspGetAllActiveIntegrationStatuses]
AS
	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Deleted] = 0
