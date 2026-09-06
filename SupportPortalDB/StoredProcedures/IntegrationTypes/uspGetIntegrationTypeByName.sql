CREATE PROCEDURE [dbo].[uspGetIntegrationTypeByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Name] = @Name
