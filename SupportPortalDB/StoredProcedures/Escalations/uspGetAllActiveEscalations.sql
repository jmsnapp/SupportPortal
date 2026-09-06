CREATE PROCEDURE [dbo].[uspGetAllActiveEscalations]
AS
	SELECT * FROM [dbo].[Escalations] WHERE [Deleted] = 0
