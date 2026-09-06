CREATE PROCEDURE [dbo].[uspGetAllActivePhases]
AS
	SELECT * FROM [dbo].[Phases] WHERE [Deleted] = 0
