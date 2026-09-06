CREATE PROCEDURE [dbo].[uspUpdateIntegrationError]
	@Id BIGINT,
	@Description NVARCHAR(255) = NULL,
	@Deleted BIT = NULL,
	@IntegrationId BIGINT = NULL,
	@ErrorMessage NVARCHAR(1027) = NULL,
	@StackTrace NVARCHAR(MAX) = NULL,
	@ErrorTime DATETIME = NULL,
	@RowVersion BINARY(8) = NULL
AS
	SET NOCOUNT ON;

	-- Optimistic concurrency. A NULL @RowVersion means the caller stated no
	-- expectation, so the write is unguarded; otherwise the row must still be at the
	-- version they read. -1 says somebody got there first, and the repository turns
	-- that into a 409 rather than silently discarding the other edit.
	IF NOT EXISTS (SELECT 1 FROM [dbo].[IntegrationErrors]
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
	UPDATE [dbo].[IntegrationErrors] SET
		[Description] = COALESCE(@Description, [Description]),
		[Deleted] = COALESCE(@Deleted, [Deleted]),
		[IntegrationId] = COALESCE(@IntegrationId, [IntegrationId]),
		[ErrorMessage] = COALESCE(@ErrorMessage, [ErrorMessage]),
		[StackTrace] = COALESCE(@StackTrace, [StackTrace]),
		[ErrorTime] = COALESCE(@ErrorTime, [ErrorTime])
	WHERE [Id] = @Id
		  AND EXISTS (
				SELECT [Description],
				       [Deleted],
				       [IntegrationId],
				       [ErrorMessage],
				       [StackTrace],
				       [ErrorTime]
				EXCEPT
				SELECT COALESCE(@Description, [Description]),
				       COALESCE(@Deleted, [Deleted]),
				       COALESCE(@IntegrationId, [IntegrationId]),
				       COALESCE(@ErrorMessage, [ErrorMessage]),
				       COALESCE(@StackTrace, [StackTrace]),
				       COALESCE(@ErrorTime, [ErrorTime])
			  );

	-- Rows affected: 0 here means the row matched but held these values already.
	SELECT CAST(@@ROWCOUNT AS BIGINT);
