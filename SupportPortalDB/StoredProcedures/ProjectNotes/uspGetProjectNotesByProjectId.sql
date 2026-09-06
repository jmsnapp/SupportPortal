CREATE PROCEDURE [dbo].[uspGetProjectNotesByProjectId]
	@ProjectId BIGINT
AS
	SELECT * FROM [dbo].[ProjectNotes] WHERE [ProjectId] = @ProjectId
