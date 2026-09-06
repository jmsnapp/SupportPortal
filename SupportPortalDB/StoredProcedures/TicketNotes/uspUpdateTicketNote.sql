CREATE PROCEDURE [dbo].[uspUpdateTicketNote]
	@Id BIGINT,
	@Description NVARCHAR(255) = NULL,
	@TicketId BIGINT = NULL,
	@Note NVARCHAR(MAX) = NULL,
	@Deleted BIT = NULL,
	@CreateTime DATETIME = NULL,
	@RowVersion BINARY(8) = NULL
AS
	SET NOCOUNT ON;

	-- Optimistic concurrency. A NULL @RowVersion means the caller stated no
	-- expectation, so the write is unguarded; otherwise the row must still be at the
	-- version they read. -1 says somebody got there first, and the repository turns
	-- that into a 409 rather than silently discarding the other edit.
	IF NOT EXISTS (SELECT 1 FROM [dbo].[TicketNotes]
				WHERE [Id] = @Id
				  AND (@RowVersion IS NULL OR [RowVersion] = @RowVersion))
	BEGIN
		SELECT CAST(-1 AS BIGINT);
		RETURN;
	END

	-- The EXCEPT guard stops an unchanged save from touching the row. SQL Server
	-- reissues RowVersion on every write, so an unconditional UPDATE would invalidate
	-- the token the caller is still holding. EXCEPT compares NULL-safely, which a
	-- chain of <> does not.
	UPDATE [dbo].[TicketNotes] SET
		[Description] = COALESCE(@Description, [Description]),
		[TicketId] = COALESCE(@TicketId, [TicketId]),
		[Note] = COALESCE(@Note, [Note]),
		[Deleted] = COALESCE(@Deleted, [Deleted]),
		[CreateTime] = COALESCE(@CreateTime, [CreateTime])
	WHERE [Id] = @Id
		  AND EXISTS (
				SELECT [Description],
				       [TicketId],
				       [Note],
				       [Deleted],
				       [CreateTime]
				EXCEPT
				SELECT COALESCE(@Description, [Description]),
				       COALESCE(@TicketId, [TicketId]),
				       COALESCE(@Note, [Note]),
				       COALESCE(@Deleted, [Deleted]),
				       COALESCE(@CreateTime, [CreateTime])
			  );

	-- Rows affected: 0 here means the row matched but held these values already.
	SELECT CAST(@@ROWCOUNT AS BIGINT);
