CREATE PROCEDURE [dbo].[uspGetProjectByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Projects] WHERE [Name] = @Name

	SELECT * FROM [dbo].[Phases] WHERE [Id] = 
		(SELECT [CurrentPhase] FROM [dbo].[Projects] WHERE [Name] = @Name)

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Projects] WHERE [Name] = @Name)

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT[CustomerId] FROM [dbo].[Projects] WHERE [Name] = @Name))

	SELECT * FROM [dbo].[ProjectNotes] WHERE [Deleted] = 0 AND [ProjectId] = 
		(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = @Name)
