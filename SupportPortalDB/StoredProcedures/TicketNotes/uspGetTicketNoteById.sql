CREATE PROCEDURE [dbo].[uspGetTicketNoteById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[TicketNotes] WHERE [Id] = @Id
