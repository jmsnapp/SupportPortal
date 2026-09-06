CREATE PROCEDURE [dbo].[uspCreateProjectNote]
	@Description NVARCHAR(255) = '',
	@ProjectId BIGINT,
	@Note NVARCHAR(MAX) = ''
AS
	INSERT INTO [dbo].[ProjectNotes] ([Description],[Deleted],[ProjectId],[Note],[CreateTime])
		VALUES (@Description,0,@ProjectId,@Note,GETUTCDATE())

	-- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
	-- identities are BIGINT, and the repositories read this with ExecuteScalar.
	SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
