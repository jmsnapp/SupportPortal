CREATE PROCEDURE [dbo].[uspGetAllActiveIntegrationTypes]
AS
	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Deleted] = 0
