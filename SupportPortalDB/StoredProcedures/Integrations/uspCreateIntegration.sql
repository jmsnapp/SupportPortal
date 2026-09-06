CREATE PROCEDURE [dbo].[uspCreateIntegration]
	@Name NVARCHAR(63),
	@Description NVARCHAR(127) = '',
	@CustomerId BIGINT,
	@IntegrationTypeId BIGINT,
	@CurrentStatusId BIGINT,
	@LastSuccessfulSync DATETIME = NULL,
	@LastFailedSync DATETIME = NULL,
	@RetryCount INT = 0
AS
	INSERT INTO [dbo].[Integrations] ([Name],[Description],[Deleted],[CustomerId],[IntegrationTypeId],[CurrentStatusId],[LastSuccessfulSync],[LastFailedSync],[RetryCount])
		VALUES (@Name,@Description,0,@CustomerId,@IntegrationTypeId, @CurrentStatusId, ISNULL(@LastSuccessfulSync,0), ISNULL(@LastFailedSync,0), @RetryCount)

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
