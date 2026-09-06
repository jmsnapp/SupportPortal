CREATE PROCEDURE [dbo].[uspGetAllActiveTicketNotes]
AS
	SELECT * FROM [dbo].[TicketNotes] WHERE [Deleted] = 0
