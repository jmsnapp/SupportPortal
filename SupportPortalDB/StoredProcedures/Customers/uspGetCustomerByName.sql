CREATE PROCEDURE [dbo].[uspGetCustomerByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Customers] WHERE [Name] = @Name

	SELECT * FROM [dbo].[Industries] WHERE [Id] = (SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Name] = @Name)
