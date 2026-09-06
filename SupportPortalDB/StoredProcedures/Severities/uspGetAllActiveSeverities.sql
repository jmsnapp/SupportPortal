CREATE PROCEDURE [dbo].[uspGetAllActiveSeverities]
AS
	SELECT * FROM [dbo].[Severities] WHERE [Deleted] = 0
