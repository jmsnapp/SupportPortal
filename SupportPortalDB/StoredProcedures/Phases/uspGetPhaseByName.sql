CREATE PROCEDURE [dbo].[uspGetPhaseByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Phases] WHERE [Name] = @Name
