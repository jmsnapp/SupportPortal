CREATE PROCEDURE [dbo].[uspGetSeverityByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Severities] WHERE [Name] = @Name
