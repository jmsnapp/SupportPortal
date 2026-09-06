CREATE PROCEDURE [dbo].[uspCreateCustomer]
	@Name NVARCHAR(63),
	@Description NVARCHAR(255) = '',
	@IndustryId BIGINT = 0,
	@PrimaryContactName NVARCHAR(63) = '',
	@PrimaryContactEmail NVARCHAR(63) = '',
	@TechnicalContactName NVARCHAR(63) = '',
	@TechnicalContactEmail NVARCHAR(63) = ''
AS
	INSERT INTO [dbo].[Customers] ([Name], [Description], [Deleted], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate])
		VALUES (@Name, @Description, 0, @IndustryId, @PrimaryContactName, @PrimaryContactEmail, @TechnicalContactName, @TechnicalContactEmail, GETUTCDATE())

    -- SCOPE_IDENTITY() as a result set, not a RETURN status: RETURN is int-typed and these
    -- identities are BIGINT, and the repositories read this with ExecuteScalar.
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
