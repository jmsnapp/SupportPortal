CREATE PROCEDURE [dbo].[uspCreateSeverity]
	@Name NVARCHAR(63),
	@Description NVARCHAR(255) = ''
AS
	INSERT INTO [dbo].[Severities] ([Name],[Description],[Deleted])
		VALUES (@Name,@Description,0)

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
