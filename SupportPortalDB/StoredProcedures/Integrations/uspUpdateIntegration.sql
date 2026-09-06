CREATE PROCEDURE [dbo].[uspUpdateIntegration]
	@Id BIGINT,
	@Name NVARCHAR(63) = NULL,
	@Description NVARCHAR(127) = NULL,
	@Deleted BIT = NULL,
	@CustomerId BIGINT = NULL,
	@IntegrationTypeId BIGINT = NULL,
	@CurrentStatusId BIGINT = NULL,
	@LastSuccessfulSync DATETIME = NULL,
	@LastFailedSync DATETIME = NULL,
	@RetryCount INT = NULL,
	@RowVersion BINARY(8) = NULL
AS
	SET NOCOUNT ON;

	-- Optimistic concurrency. A NULL @RowVersion means the caller stated no
	-- expectation, so the write is unguarded; otherwise the row must still be at the
	-- version they read. -1 says somebody got there first, and the repository turns
	-- that into a 409 rather than silently discarding the other edit.
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Integrations]
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
	UPDATE [dbo].[Integrations] SET
		[Name] = COALESCE(@Name, [Name]),
		[Description] = COALESCE(@Description, [Description]),
		[Deleted] = COALESCE(@Deleted, [Deleted]),
		[CustomerId] = COALESCE(@CustomerId, [CustomerId]),
		[IntegrationTypeId] = COALESCE(@IntegrationTypeId, [IntegrationTypeId]),
		[CurrentStatusId] = COALESCE(@CurrentStatusId, [CurrentStatusId]),
		[LastSuccessfulSync] = COALESCE(@LastSuccessfulSync, [LastSuccessfulSync]),
		[LastFailedSync] = COALESCE(@LastFailedSync, [LastFailedSync]),
		[RetryCount] = COALESCE(@RetryCount, [RetryCount])
	WHERE [Id] = @Id
		  AND EXISTS (
				SELECT [Name],
				       [Description],
				       [Deleted],
				       [CustomerId],
				       [IntegrationTypeId],
				       [CurrentStatusId],
				       [LastSuccessfulSync],
				       [LastFailedSync],
				       [RetryCount]
				EXCEPT
				SELECT COALESCE(@Name, [Name]),
				       COALESCE(@Description, [Description]),
				       COALESCE(@Deleted, [Deleted]),
				       COALESCE(@CustomerId, [CustomerId]),
				       COALESCE(@IntegrationTypeId, [IntegrationTypeId]),
				       COALESCE(@CurrentStatusId, [CurrentStatusId]),
				       COALESCE(@LastSuccessfulSync, [LastSuccessfulSync]),
				       COALESCE(@LastFailedSync, [LastFailedSync]),
				       COALESCE(@RetryCount, [RetryCount])
			  );

	-- Rows affected: 0 here means the row matched but held these values already.
	SELECT CAST(@@ROWCOUNT AS BIGINT);
