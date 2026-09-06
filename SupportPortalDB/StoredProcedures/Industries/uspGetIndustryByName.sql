CREATE PROCEDURE [dbo].[uspGetIndustryByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Industries] WHERE [Name] = @Name
