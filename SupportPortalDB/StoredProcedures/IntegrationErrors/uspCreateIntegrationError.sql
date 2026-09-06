CREATE PROCEDURE [dbo].[uspCreateIntegrationError]
	@Description NVARCHAR(255) = '',
	@IntegrationId BIGINT,
	@ErrorMessage NVARCHAR(1027) = '',
	@StackTrace NVARCHAR(MAX) = '',
	@ErrorTime DATETIME = NULL
AS
	INSERT INTO [dbo].[IntegrationErrors] ([Description],[Deleted],[IntegrationId],[ErrorMessage],[StackTrace],[ErrorTime])
		VALUES (@Description,0,@IntegrationId,@ErrorMessage,@StackTrace,ISNULL(@ErrorTime,GETUTCDATE()))

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
