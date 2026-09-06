CREATE PROCEDURE [dbo].[uspGetCustomerById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Customers] WHERE [Id] = @Id

	SELECT * FROM [dbo].[Industries] WHERE [Id] = (SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = @Id)
