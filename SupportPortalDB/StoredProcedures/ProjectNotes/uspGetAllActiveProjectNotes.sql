CREATE PROCEDURE [dbo].[uspGetAllActiveProjectNotes]
AS
	SELECT * FROM [dbo].[ProjectNotes] WHERE [Deleted] = 0
