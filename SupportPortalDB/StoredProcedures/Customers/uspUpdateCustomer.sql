CREATE PROCEDURE [dbo].[uspUpdateCustomer]
	@Id BIGINT,
	@Name NVARCHAR(63) = NULL,
	@Description NVARCHAR(255) = NULL,
	@Deleted BIT = NULL,
	@IndustryId BIGINT = NULL,
	@PrimaryContactName NVARCHAR(63) = NULL,
	@PrimaryContactEmail NVARCHAR(63) = NULL,
	@TechnicalContactName NVARCHAR(63) = NULL,
	@TechnicalContactEmail NVARCHAR(63) = NULL,
	@RowVersion BINARY(8) = NULL
AS
	SET NOCOUNT ON;

	-- Optimistic concurrency. A NULL @RowVersion means the caller stated no
	-- expectation, so the write is unguarded; otherwise the row must still be at the
	-- version they read. -1 says somebody got there first, and the repository turns
	-- that into a 409 rather than silently discarding the other edit.
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers]
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
	UPDATE [dbo].[Customers] SET
		[Name] = COALESCE(@Name, [Name]),
		[Description] = COALESCE(@Description, [Description]),
		[Deleted] = COALESCE(@Deleted, [Deleted]),
		[IndustryId] = COALESCE(@IndustryId, [IndustryId]),
		[PrimaryContactName] = COALESCE(@PrimaryContactName, [PrimaryContactName]),
		[PrimaryContactEmail] = COALESCE(@PrimaryContactEmail, [PrimaryContactEmail]),
		[TechnicalContactName] = COALESCE(@TechnicalContactName, [TechnicalContactName]),
		[TechnicalContactEmail] = COALESCE(@TechnicalContactEmail, [TechnicalContactEmail])
	WHERE [Id] = @Id
		  AND EXISTS (
				SELECT [Name],
				       [Description],
				       [Deleted],
				       [IndustryId],
				       [PrimaryContactName],
				       [PrimaryContactEmail],
				       [TechnicalContactName],
				       [TechnicalContactEmail]
				EXCEPT
				SELECT COALESCE(@Name, [Name]),
				       COALESCE(@Description, [Description]),
				       COALESCE(@Deleted, [Deleted]),
				       COALESCE(@IndustryId, [IndustryId]),
				       COALESCE(@PrimaryContactName, [PrimaryContactName]),
				       COALESCE(@PrimaryContactEmail, [PrimaryContactEmail]),
				       COALESCE(@TechnicalContactName, [TechnicalContactName]),
				       COALESCE(@TechnicalContactEmail, [TechnicalContactEmail])
			  );

	-- Rows affected: 0 here means the row matched but held these values already.
	SELECT CAST(@@ROWCOUNT AS BIGINT);
