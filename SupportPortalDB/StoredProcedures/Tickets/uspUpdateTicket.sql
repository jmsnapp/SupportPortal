CREATE PROCEDURE [dbo].[uspUpdateTicket]
	@Id BIGINT,
	@Description NVARCHAR(1023) = NULL,
	@Deleted BIT = NULL,
	@CustomerId BIGINT = NULL,
	@IntegrationId BIGINT = NULL,
	@Reproduce NVARCHAR(MAX) = NULL,
	@SeverityId BIGINT = NULL,
	@StatusId BIGINT = NULL,
	@ReportedBy NVARCHAR(63) = NULL,
	@AssignedTo NVARCHAR(63) = NULL,
	@ResolutionDate DATETIME = NULL,
	@Resolution NVARCHAR(MAX) = NULL,
	@EscalationId BIGINT = NULL,
	@RowVersion BINARY(8) = NULL
AS
	SET NOCOUNT ON;

	-- Optimistic concurrency. A NULL @RowVersion means the caller stated no
	-- expectation, so the write is unguarded; otherwise the row must still be at the
	-- version they read. -1 says somebody got there first, and the repository turns
	-- that into a 409 rather than silently discarding the other edit.
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Tickets]
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
	UPDATE [dbo].[Tickets] SET
		[Description] = COALESCE(@Description, [Description]),
		[Deleted] = COALESCE(@Deleted, [Deleted]),
		[CustomerId] = COALESCE(@CustomerId, [CustomerId]),
		[IntegrationId] = COALESCE(@IntegrationId, [IntegrationId]),
		[Reproduce] = COALESCE(@Reproduce, [Reproduce]),
		[SeverityId] = COALESCE(@SeverityId, [SeverityId]),
		[StatusId] = COALESCE(@StatusId, [StatusId]),
		[ReportedBy] = COALESCE(@ReportedBy, [ReportedBy]),
		[AssignedTo] = COALESCE(@AssignedTo, [AssignedTo]),
		[ResolutionDate] = COALESCE(@ResolutionDate, [ResolutionDate]),
		[Resolution] = COALESCE(@Resolution, [Resolution]),
		[EscalationId] = COALESCE(@EscalationId, [EscalationId])
	WHERE [Id] = @Id
		  AND EXISTS (
				SELECT [Description],
				       [Deleted],
				       [CustomerId],
				       [IntegrationId],
				       [Reproduce],
				       [SeverityId],
				       [StatusId],
				       [ReportedBy],
				       [AssignedTo],
				       [ResolutionDate],
				       [Resolution],
				       [EscalationId]
				EXCEPT
				SELECT COALESCE(@Description, [Description]),
				       COALESCE(@Deleted, [Deleted]),
				       COALESCE(@CustomerId, [CustomerId]),
				       COALESCE(@IntegrationId, [IntegrationId]),
				       COALESCE(@Reproduce, [Reproduce]),
				       COALESCE(@SeverityId, [SeverityId]),
				       COALESCE(@StatusId, [StatusId]),
				       COALESCE(@ReportedBy, [ReportedBy]),
				       COALESCE(@AssignedTo, [AssignedTo]),
				       COALESCE(@ResolutionDate, [ResolutionDate]),
				       COALESCE(@Resolution, [Resolution]),
				       COALESCE(@EscalationId, [EscalationId])
			  );

	-- Rows affected: 0 here means the row matched but held these values already.
	SELECT CAST(@@ROWCOUNT AS BIGINT);
