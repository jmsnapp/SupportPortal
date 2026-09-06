CREATE PROCEDURE [dbo].[uspCreateTicketNote]
	@Description NVARCHAR(255) = '',
	@TicketId BIGINT,
	@Note NVARCHAR(MAX) = ''
AS
	INSERT INTO [dbo].[TicketNotes] ([Description], [TicketId], [Note], [Deleted], [CreateTime])
		VALUES (@Description, @TicketId, @Note, 0, GETUTCDATE())

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
