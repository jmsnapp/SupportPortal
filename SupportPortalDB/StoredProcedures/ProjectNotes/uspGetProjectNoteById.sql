CREATE PROCEDURE [dbo].[uspGetProjectNoteById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[ProjectNotes]  WHERE [Id] = @Id
