CREATE PROCEDURE [dbo].[uspGetIntegrationStatusByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Name] = @Name
