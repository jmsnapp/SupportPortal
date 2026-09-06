CREATE PROCEDURE [dbo].[uspCreateLinkProjectPhase]
	@Description NVARCHAR(255) = '',
	@ProjectId BIGINT,
	@PhaseId BIGINT,
	@Percentage DECIMAL(5,2) = 0,
	@Order INT = 0
AS
	INSERT INTO [dbo].[LinkProjectPhases] ([Description],[Deleted],[ProjectId],[PhaseId],[Percentage],[Order])
		VALUES (@Description,0,@ProjectId,@PhaseId,@Percentage,@Order)

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
