CREATE PROCEDURE [dbo].[uspCreateTicket]
	@Description NVARCHAR(1023) = '',
	@CustomerId BIGINT,
	@IntegrationId BIGINT,
	@Reproduce NVARCHAR(MAX) = '',
	@SeverityId BIGINT,
	@StatusId BIGINT,
	@ReportedBy NVARCHAR(63) = '',
	@AssignedTo NVARCHAR(63) = ''
AS
	INSERT INTO [dbo].[Tickets] ([Description],[Deleted],[CustomerId],[IntegrationId],[Reproduce],[SeverityId],[StatusId],[ReportedBy],[AssignedTo],[CreatedDate],[ResolutionDate],[Resolution],[EscalationId])
		VALUES (@Description,0,@CustomerId,@IntegrationId,@Reproduce,@SeverityId,@StatusId,@ReportedBy,@AssignedTo,GETUTCDATE(),0,'',0)

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
