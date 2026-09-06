CREATE PROCEDURE [dbo].[uspGetTicketNotesByTicketId]
	@TicketId BIGINT
AS
	SELECT * FROM [dbo].[TicketNotes] WHERE [TicketId] = @TicketId
