CREATE PROCEDURE [dbo].[uspCreateProject]
	@Name NVARCHAR(63),
	@Description NVARCHAR(255) = '',
	@CustomerId BIGINT,
	@CurrentPhase BIGINT,
	@TargetGoLiveDate DATETIME = NULL,
	@ActualGoLiveDate DATETIME = NULL
AS
	INSERT INTO [dbo].[Projects] ([Name],[Description],[Deleted],[CustomerId],[CurrentPhase],[TargetGoLiveDate],[ActualGoLiveDate])
		VALUES (@Name,@Description,0,@CustomerId,@CurrentPhase,ISNULL(@TargetGoLiveDate,0),ISNULL(@ActualGoLiveDate,0))

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
