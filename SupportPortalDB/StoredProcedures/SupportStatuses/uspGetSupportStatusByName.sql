CREATE PROCEDURE [dbo].[uspGetSupportStatusByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[SupportStatuses] WHERE [Name] = @Name
