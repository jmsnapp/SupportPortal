CREATE PROCEDURE [dbo].[uspCreateEscalation]
	@Description NVARCHAR(255) = '',
	@ProblemSummary NVARCHAR(MAX) = '',
	@CustomerImpact NVARCHAR(MAX) = '',
	@RootCause NVARCHAR(MAX) = '',
	@RecommendedActions NVARCHAR(MAX) = ''
AS
	INSERT INTO [dbo].[Escalations] ([Description],[Deleted],[CreatedDate],[ProblemSummary],[CustomerImpact],[RootCause],[RecommendedActions])
		VALUES (@Description,0,GETUTCDATE(),@ProblemSummary,@CustomerImpact,@RootCause,@RecommendedActions)

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
